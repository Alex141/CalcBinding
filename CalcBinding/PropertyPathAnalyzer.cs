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
        private string _str;

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
                var token = ReadNextToken();

                var res = NextStep(token);

                if (!res)
                {
                    throw new Exception(String.Format("PropertyPathAnalyzer: unexpected token '{0}', start = {1} end = {2}", token.Value, token.Start, token.End));
                }

                if (token.IsEmpty)
                    break;

            } while (true);

            return _pathTokens;
        }

        private Token ReadNextToken()
        {
            // nested parser?

            while (UnknownDelimiters.Contains(_str[_position].ToString()))
            {
                _position++;

                var delimToken = ReadKnownDelimiter();
                if (delimToken != null)
                    return delimToken;
            }

            // we read word
            var startPosition = _position;

            while (!delimiters.Contains(_str[_position].ToString()))
            {
                _position++;
                if (_position > _str.Length - 1)
                    break;
            }

            var word = _str.Substring(startPosition, _position - startPosition);

            return new Token(TokenType.Identifier, word);
        }

        private Token ReadKnownDelimiter()
        {
            if (_position > _str.Length - 1)
                return new Token(TokenType.Empty, string.Empty);

            if (_str[_position] == '.')
            {
                _position++;
                return new Token(TokenType.Dot, _str[_position].ToString());
            }

            if (_str[_position] == ':')
            {
                _position++;
                return new Token(TokenType.Colon, _str[_position].ToString());
            }

            return null;
        }

        private bool NextStep()
        {
            return NextStep(ReadNextToken());
        }

        private bool NextStep(Token token)
        {
            if (_state == State.Initial)
            {
                if (token.IsIdentifier && token.Value == "Math")
                {
                    _state = State.MathClass;
                    return true;
                }
                else if (token.IsIdentifier)
                {
                    _state = State.Identifier;
                    return true;
                }
                return false;
            }

            if (_state == State.MathClass)
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

            if (_state == State.Identifier)
            {
                if (token.IsColon)
                {
                    namespaceToken = token;

                    token = ReadNextToken();

                    if (token.IsIdentifier)
                    {
                        classToken = token;

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
                    _state = State.PropPathDot;
                    return true;
                    // todo member tokens??
                }
                else
                {
                    // здесь надо выводить PropertyPath от первого символа в initial до token.Start - 1
                    _state = State.Initial;
                    return true;
                }
                return false;
            }

            if (_state == State.StaticPropPathDot)
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
                        //здесь надо выводить enum или static property в зависимости от набранных токенов и определения типа
                        _state = State.Initial;
                        return true;
                    }
                }

                return false;
            }

            if (_state == State.PropPathDot)
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
                        _state = State.Initial;
                        return true;
                    }
                }
                return false;
            }

            _lastErrorMessage = String.Format("PropertyPathAnalyzer: State {0} is not supported", _state);
            return false;
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

            public Token(TokenType type, string value)
            {
                TokenType = type;
                Value = value;
            }
        } 

        #endregion
    }

    public class PathToken
    {
        public PathTokenType PathType { get { return Id.PathType; } }

        public string Value { get { return Id.Value; } }

        public readonly PathTokenId Id { get; private set; }

        public int Start { get; private set; }

        public int End { get; private set; }

        public string ClassName { get; private set; }

        public IEnumerable<string> MembersList { get; private set; }

        /// <summary>
        /// Type of Enum if PathToken represents enum path, e.g. MyEnum.MyValue, and null otherwise
        /// </summary>
        public readonly Type EnumType { get; private set; }

        public PathToken(PathTokenType pathType, string value)
        {
            Id = new PathTokenId(pathType, value);
        }

        public PathToken (Type enumType, string value)
        {
            EnumType = enumType;
            Id = new PathTokenId(PathTokenType.Enum, value);
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
