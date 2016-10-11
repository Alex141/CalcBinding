using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CalcBinding
{
    public class PropertyPathAnalyzer
    {
        public static string[] UnknownDelimiters = new [] 
            { 
                "(", ")", "+", "-", "*", "/", "%", "^", "&&", "||", 
                "&", "|", "?", "<=", ">=", "<", ">", "==", "!=", "!", "," 
            };

        public static string[] KnownDelimiters = new[]
            {
                ".", ":"
            };

        private static string[] delimiters;

        private int _position;
        private State _state;
        private SubState _subState;
        private string _str;

        private List<PathToken> _pathTokens;
        private string namespaceIdentifier;
        private string classIdentifier;
        private List<string> propertyPathIdentifiers;
        private string _lastIdentifier;

        static PropertyPathAnalyzer()
        {
            delimiters = KnownDelimiters.Concat(UnknownDelimiters).ToArray();
        }

        public List<PathToken> GetPathes(string normPath)
        {
            _state = State.Initial;
            _position = 0;
            _pathTokens = new List<PathToken>();
            _str = normPath;

            do
            {
                var startTokenPosition = _position;

                var token = ReadNextToken();

                var endTokenPosition = _position;

                var res = NextStep(token);

                if (!res)
                {
                    throw new NotSupportedException(String.Format("PropertyPathAnalyzer: unsupported token '{0}', start = {1} end = {2}", 
                        token.Value, startTokenPosition, endTokenPosition));
                }

                if (token.IsEmpty)
                    break;

            } while (true);

            return _pathTokens;
        }

        private Token ReadNextToken()
        {
           _subState = SubState.Initial;

            while (true)
            {
                Symbol symbol = _position < _str.Length ? (Symbol)_str[_position] : Symbol.End;
                var token = ReadToken(symbol);

                _position++;
                if (token != null || symbol.IsEnd)
                {
                    return token ?? Token.Empty;
                }
            }
        }

        private int _identifierStartPos;

        private Token ReadToken(Symbol symbol)
        {
            switch (_subState)
            {
                case SubState.Initial: 
                    
                    if (symbol == '.')
                        return Token.Dot;
                    
                    if (symbol == ':')
                        return Token.Colon;

                    if (symbol == '"')
                    {
                        _subState = SubState.String;
                        return null;
                    }

                    if (symbol.IsEnd)
                        return Token.Empty;

                    if (UnknownDelimiters.Contains(symbol))
                    {
                        return null;
                    }

                    _identifierStartPos = _position;
                    _subState = SubState.Identifier;
                    return null;

                case SubState.Identifier:
                    if (symbol.IsEnd || delimiters.Contains(symbol))
                    {
                        var identifier = new Token(TokenType.Identifier, _str.Substring(_identifierStartPos, _position - _identifierStartPos));

                        _subState = SubState.Initial;
                        return identifier;
                    }

                    return null;

                case SubState.String:
                    if (symbol == '"')
                    {
                        _subState = SubState.Initial;
                        return null;
                    }

                    if (symbol.IsEnd)
                    {
                        throw new NotSupportedException("string constant in property path hasn't end quotes");
                    }
                    return null;

                default:
                    throw new NotSupportedException(string.Format("Identifier lexer: unexpected state '{0}'", _subState));
            }

        }
            // nested parser?

        //    while (UnknownDelimiters.Contains(_str[_position].ToString()))
        //    {
        //        _position++;

        //        var delimToken = ReadKnownDelimiter();
        //        if (delimToken != null)
        //            return delimToken;
        //    }

        //    // we read word
        //    var startPosition = _position;

        //    while (!delimiters.Contains(_str[_position].ToString()))
        //    {
        //        _position++;
        //        if (_position > _str.Length - 1)
        //            break;
        //    }

        //    var word = _str.Substring(startPosition, _position - startPosition);

        //    return new Token(TokenType.Identifier, word);
        //}

        //private Token ReadKnownDelimiter()
        //{
        //    if (_position > _str.Length - 1)
        //        return new Token(TokenType.Empty, string.Empty);

        //    if (_str[_position] == '.')
        //    {
        //        _position++;
        //        return new Token(TokenType.Dot, _str[_position].ToString());
        //    }

        //    if (_str[_position] == ':')
        //    {
        //        _position++;
        //        return new Token(TokenType.Colon, _str[_position].ToString());
        //    }

        //    return null;
        //}

        //private bool NextStep()
        //{
        //    return NextStep(ReadNextToken());
        //}

        private bool NextStep(Token token)
        {
            switch (_state)
            {
                case State.Initial:
                    {
                        if (token.IsIdentifier && token.Value == "Math")
                        {
                            _state = State.MathClass;
                            return true;
                        }
                        else if (token.IsIdentifier)
                        {
                            _lastIdentifier = token.Value;
                            _state = State.Identifier;
                            return true;
                        }
                        return false;
                    }
                case State.MathClass:
                    {
                        if (token.IsDot)
                        {
                            token = ReadNextToken();

                            if (token.IsIdentifier)
                            {
                                // здесь надо вывести мат. класс
                                _state = State.Initial;
                                return true;
                            }
                        }
                        return false;
                    }
                case State.Identifier:
                    {
                        if (token.IsColon)
                        {
                            namespaceIdentifier = token.Value;

                            token = ReadNextToken();

                            if (token.IsIdentifier)
                            {
                                classIdentifier = token.Value;

                                token = ReadNextToken();

                                if (token.IsDot)
                                {
                                    _state = State.StaticPropPathDot;
                                    return true;
                                }
                            }
                        }
                        else if (token.IsDot)
                        {
                            propertyPathIdentifiers.Add(_lastIdentifier);
                            _state = State.PropPathDot;
                            return true;
                        }
                        else
                        {
                            propertyPathIdentifiers.Add(_lastIdentifier);
                            _pathTokens.Add(new PathToken(PathTokenType.Property, ))
                            // здесь надо выводить PropertyPath от первого символа в initial до token.Start - 1
                            propertyPathIdentifiers.Clear();
                            _state = State.Initial;
                            return true;
                        }
                        return false;
                    }
                case State.StaticPropPathDot:
                    {
                        if (token.IsIdentifier)
                        {
                            token = ReadNextToken();

                            if (token.IsDot)
                            {
                                propertyPathIdentifiers.Add(token.Value);
                                // state unchanged
                                return true;
                            }
                            else
                            {
                                //здесь надо выводить enum или static property в зависимости от набранных токенов и определения типа
                                propertyPathIdentifiers.Clear();
                                _state = State.Initial;
                                return true;
                            }
                        }
                        return false;
                    }
                case State.PropPathDot:
                    {
                        if (token.IsIdentifier)
                        {
                            token = ReadNextToken();

                            if (token.IsDot)
                            {
                                // state unchanged
                                return true;
                            }
                            else
                            {
                                // здесь надо выводить PropertyPath от первого символа до token.Start - 1
                                propertyPathIdentifiers.Clear();
                                _state = State.Initial;
                                return true;
                            }
                        }
                        return false;
                    }
                default:
                    throw new NotSupportedException(String.Format("PropertyPathAnalyzer: State {0} is not supported", _state));
            }
        }

        #region Nested types

        enum State
        {
            Initial,
            MathClass,
            Identifier,
            StaticPropPathDot,
            PropPathDot
        }

        enum SubState
        {
            Initial,
            Identifier,
            String
        }

        enum TokenType
        {
            Identifier,
            Dot,
            Colon,
            Empty
        }

        class Token
        {
            public readonly TokenType TokenType { get; private set; }

            public readonly string Value { get; private set; }

            public static Token Empty(int position)
            {
                return new Token(TokenType.Empty, null, position, position);
            }

            public static Token Dot(int position)
            {
                return new Token(TokenType.Dot, ".", position, position);
            }

            public static Token Colon(int position)
            {
                return new Token(TokenType.Colon, ":", position, position);
            }

            public static Token Identifier(string identifier, int startPosition, int endPosition)
            {
                return new Token(TokenType.Identifier, identifier, startPosition, endPosition);
            }

            public bool IsIdentifier
            {
                get
                {
                    return TokenType == PropertyPathAnalyzer.TokenType.Identifier;
                }
            }

            public bool IsDot
            {
                get
                {
                    return TokenType == PropertyPathAnalyzer.TokenType.Dot;
                }
            }

            public bool IsColon
            {
                get
                {
                    return TokenType == PropertyPathAnalyzer.TokenType.Colon;
                }
            }

            public bool IsEmpty
            {
                get
                {
                    return TokenType == PropertyPathAnalyzer.TokenType.Empty;
                }
            }

            public  int Start { get; private set; }
            public  int End { get; private set; }

            private Token(TokenType type, string value, int startPosition, int endPosition)
            {
                TokenType = type;
                Value = value;
                Start = startPosition;
                End = endPosition;
            }
        } 

        class Symbol
        {
            private char _c;
            public bool IsEnd { get; private set; }
            public static readonly Symbol End = new Symbol() { IsEnd = true };

            protected Symbol()
            {

            }

            public Symbol(char c)
            {
                _c = c;
                IsEnd = false;
            }

            public implicit operator Symbol(Char c)
            {
                return new Symbol(c);
            }

            public implicit operator Char(Symbol symbol)
            {
                if (symbol.IsEnd)
                    throw new NotSupportedException("Symbol to char: End symbol couldn't be translated to char");

                return symbol._c;
            }

            public implicit operator String(Symbol symbol)
            {
                return new String(new []{(Char)symbol});
            }

            public static bool operator ==(Symbol symbol, Char c)
            {
                if (symbol.IsEnd)
                    return false;

                return symbol._c == c;
            }
        }
        #endregion
    }

    public abstract class PathToken
    {
        public int Start { get; private set; }

        public int End { get; private set; }

        public PathToken (int start, int end)
        {
            Start = start;
            End = end;
        }
    }

    public class PropertyPathToken:PathToken
    {
        public IEnumerable<string> Properties { get; private set; }
        public PropertyPathToken(int start, int end, List<string> properties):base(start, end)
        {
            Properties = properties;
        }
    }

    public class StaticPropertyPathToken:PropertyPathToken
    {
        public string Class { get; private set; }
        public StaticPropertyPathToken(int start, int end, string @class, List<string> properties):base(start, end, properties)
        {
            Class = @class;
        }
    }

    public class EnumToken:PathToken
    {
        public Type Enum { get; private set; }
        public EnumToken(int start, int end, Type @enum):base(start, end)
        {
            Enum = @enum;
        }
    }

    public class MathToken:PathToken
    {
        public string MathMember { get; private set; }

        public MathToken(int start, int end, string mathMember):base(start, end)
        {
            MathMember = mathMember;
        }
    }

    public class PathTokenId
    {
        public readonly PathTokenType PathType { get; private set; }
        public readonly string Value { get; private set; }

        public PathTokenId(PathTokenType pathType, string value)
        {
            PathType = pathType;
            Value = value;
        }
    }

    public enum PathTokenType
    {
        /// <summary>
        /// Math, e.g. Math.Sin, Math.PI
        /// </summary>
        Math,
        /// <summary>
        /// Usual propertyPath, e.g. Name, Caption, Models.Count
        /// </summary>
        Property,
        /// <summary>
        /// Static propertyPatj, e.g. local:MyStaticVM.MyProp
        /// </summary>
        StaticProperty,
        /// <summary>
        /// Enum, e.g. local:MyEnum.MyValue
        /// </summary>
        Enum
    }
}
