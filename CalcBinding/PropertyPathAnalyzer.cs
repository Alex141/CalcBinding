using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Markup;

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
        private Token namespaceIdentifier;
        private Token classIdentifier;
        private List<Token> propertyPathIdentifiers;
        private Token _lastIdentifier;

        static PropertyPathAnalyzer()
        {
            delimiters = KnownDelimiters.Concat(UnknownDelimiters).ToArray();
        }

        public List<PathToken> GetPathes(string normPath, IXamlTypeResolver typeResolver)
        {
            _typeResolver = typeResolver;
            _state = State.Initial;
            _position = 0;
            _pathTokens = new List<PathToken>();
            _str = normPath;
            propertyPathIdentifiers = new List<Token>();

            Trace.WriteLine(string.Format("PropertyPathAnalyzer.GetPathes: start read {0} ", normPath));
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

            Trace.WriteLine(string.Format("PropertyPathAnalyzer.GetPathes: end read {0} ", normPath));
            Trace.WriteLine(string.Format("PropertyPathAnalyzer.GetPathes: tokens: {0}", String.Join("", _pathTokens.Select(pt => string.Format("\n{0} ({1})", pt.Id.Value, pt.Id.PathType)))));

            return _pathTokens;
        }

        private Token ReadNextToken()
        {
           _subState = SubState.Initial;

            while (true)
            {
                Symbol symbol = _position < _str.Length ? (Symbol)_str[_position] : Symbol.End;
                var token = ReadToken(symbol);

                if (token != null || symbol.IsEnd)
                {
                    var resultToken = token ?? Token.Empty(_position);
                    Trace.WriteLine(string.Format("PropertyPathAnalyzer: read {0} ({1})", resultToken.Value, resultToken.TokenType));
                    return resultToken;
                }
                _position++;
            }
        }

        private int _identifierStartPos;
        private IXamlTypeResolver _typeResolver;

        private Token ReadToken(Symbol symbol)
        {
            switch (_subState)
            {
                case SubState.Initial:

                    if (symbol == '.')
                    {
                        _position++;
                        return Token.Dot(_position);
                    }

                    if (symbol == ':')
                    {
                        _position++;
                        return Token.Colon(_position);
                    }

                    if (symbol == '"')
                    {
                        _subState = SubState.String;
                        return null;
                    }

                    if (symbol.IsEnd)
                        return Token.Empty(_position);

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
                        var identifier = Token.Identifier(_str.Substring(_identifierStartPos, _position - _identifierStartPos), _identifierStartPos, _position);

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
            
        private bool NextStep(Token token)
        {
            switch (_state)
            {
                case State.Initial:
                    {
                        if (token.IsIdentifier && token.Value == "Math")
                        {
                            _lastIdentifier = token;
                            _state = State.MathClass;
                            return true;
                        }
                        else if (token.IsIdentifier)
                        {
                            _lastIdentifier = token;
                            _state = State.Identifier;
                            return true;
                        }
                        else if (token.IsEmpty)
                            return true;

                        return false;
                    }
                case State.MathClass:
                    {
                        if (token.IsDot)
                        {
                            token = ReadNextToken();

                            if (token.IsIdentifier)
                            {
                                // math member output
                                var mathToken = new MathToken(_lastIdentifier.Start, token.End, token.Value);
                                _pathTokens.Add(mathToken);

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
                            namespaceIdentifier = _lastIdentifier;

                            token = ReadNextToken();

                            if (token.IsIdentifier)
                            {
                                classIdentifier = token;

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
                            // property path output
                            _pathTokens.Add(new PropertyPathToken(token.Start, token.End, propertyPathIdentifiers.Select(i => i.Value)));
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
                            propertyPathIdentifiers.Add(token);
                            _lastIdentifier = token;

                            token = ReadNextToken();

                            if (token.IsDot)
                            {
                                // state unchanged
                                return true;
                            }
                            else
                            {
                                PathToken pathToken;
                                Type enumType;
                                string typeFullName = substr(namespaceIdentifier.Start, classIdentifier.End);
                                if (propertyPathIdentifiers.Count == 1 && ((enumType = TakeEnum(typeFullName)) != null))
                                {
                                    // enum output
                                    var enumMember = propertyPathIdentifiers.Single();
                                    pathToken = new EnumToken(namespaceIdentifier.Start, enumMember.End, namespaceIdentifier.Value, enumType, enumMember.Value);
                                }
                                else
                                {
                                    //static property path output
                                    pathToken = new StaticPropertyPathToken(namespaceIdentifier.Start, _lastIdentifier.End, namespaceIdentifier.Value, classIdentifier.Value, propertyPathIdentifiers.Select(i => i.Value));
                                }
                                _pathTokens.Add(pathToken);

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
                                // property path output
                                _pathTokens.Add(new PropertyPathToken(token.Start, token.End, propertyPathIdentifiers.Select(i => i.Value)));
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

        private string substr(int start, int end)
        {
            return _str.Substring(start, end - start);
        }

        /// <summary>
        /// Found out whether xaml namespace:class is enum class or not. If yes, return enum type, otherwise - null 
        /// </summary>
        /// <param name="namespace"></param>
        /// <param name="class"></param>
        /// <returns></returns>
        private Type TakeEnum(string fullTypeName)
        {
            var @type = _typeResolver.Resolve(fullTypeName);

            if (@type != null && @type.IsEnum)
                return @type;
            return null;
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
            public TokenType TokenType { get; private set; }

            public string Value { get; private set; }

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

            public static implicit operator Symbol(Char c)
            {
                return new Symbol(c);
            }

            public static implicit operator Char(Symbol symbol)
            {
                if (symbol.IsEnd)
                    throw new NotSupportedException("Symbol to char: End symbol couldn't be translated to char");

                return symbol._c;
            }

            public static implicit operator String(Symbol symbol)
            {
                return new String(new []{(Char)symbol});
            }

            public static bool operator ==(Symbol symbol, Char c)
            {
                if (symbol.IsEnd)
                    return false;

                return symbol._c == c;
            }

            public static bool operator !=(Symbol symbol, Char c)
            {
                if (symbol.IsEnd)
                    return true;

                return symbol._c != c;
            }
        }
        #endregion
    }

    public abstract class PathToken
    {
        public int Start { get; private set; }

        public int End { get; private set; }

        public abstract PathTokenId Id { get; }

        protected PathToken (int start, int end)
        {
            Start = start;
            End = end;
        }
    }

    public class PropertyPathToken:PathToken
    {
        public IEnumerable<string> Properties { get; private set; }

        private PathTokenId id;
        public override PathTokenId Id { get { return id; } }

        public PropertyPathToken(int start, int end, IEnumerable<string> properties):base(start, end)
        {
            Properties = properties.ToList();
            id = new PathTokenId(PathTokenType.Property, String.Join(".", Properties));
        }
    }

    public class StaticPropertyPathToken:PropertyPathToken
    {
        public string Class { get; private set; }
        public string Namespace { get; private set; }

        private PathTokenId id;
        public override PathTokenId Id { get { return id; } }

        public StaticPropertyPathToken(int start, int end, string @namespace, string @class, IEnumerable<string> properties):base(start, end, properties)
        {
            Class = @class;
            Namespace = @namespace;
            id = new PathTokenId(PathTokenType.StaticProperty,String.Format("{0}:{1}.{2}", Namespace, Class, String.Join(".", Properties)));
        }
    }

    public class EnumToken:PathToken
    {
        public Type Enum { get; private set; }
        public string EnumMember { get; private set; }
        public string Namespace { get; private set; }

        private PathTokenId id;
        public override PathTokenId Id { get { return id; } }

        public EnumToken(int start, int end, string @namespace, Type @enum, string enumMember):base(start, end)
        {
            Enum = @enum;
            EnumMember = enumMember;
            Namespace = @namespace;

            id = new PathTokenId(PathTokenType.StaticProperty, String.Format("{0}:{1}.{2}", Namespace, @enum.Name, EnumMember));
        }
    }

    public class MathToken:PathToken
    {
        public string MathMember { get; private set; }

        private PathTokenId id;
        public override PathTokenId Id { get { return id; } }

        public MathToken(int start, int end, string mathMember):base(start, end)
        {
            MathMember = mathMember;
            id = new PathTokenId(PathTokenType.StaticProperty, String.Join(".", "Math", MathMember));
        }
    }

    public class PathTokenId
    {
        public PathTokenType PathType { get; private set; }
        public string Value { get; private set; }

        public PathTokenId(PathTokenType pathType, string value)
        {
            PathType = pathType;
            Value = value;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var o = obj as PathTokenId;

            if (o == null)
                return false;

            return (o.PathType == PathType && o.Value == Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode() ^ PathType.GetHashCode();
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
