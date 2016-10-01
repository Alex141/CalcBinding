using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CalcBinding
{
    public class PropertyPathAnalyzer
    {
        private const string[] operators = new[] 
            { 
                "(", ")", "+", "-", "*", "/", "%", "^", "&&", "||", 
                "&", "|", "?", ":", "<=", ">=", "<", ">", "==", "!=", "!", "," 
            };

        private int _position;
        private State _state;

        public List<PathToken> GetPathes(string normPath)
        {
            _state = State.Initial;
            _position = 0;
            _pathTokens = new List<PathToken>();

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
            throw new NotImplementedException();
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
        public readonly PathTokenType PathType { get; private set; }

        public uint Start { get; private set; }

        public uint End { get; private set; }

        public string Value { get; private set; }

        public string ClassName { get; private set; }

        public IEnumerable<string> MembersList { get; private set; }

        public PathToken(PathTokenType pathType)
        {
            PathType = pathType;
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
