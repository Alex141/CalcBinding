using System.Collections.Generic;
using CalcBinding.ExpressionParsers;
using DynamicExpresso;

namespace Tests.Mocks
{
    public sealed class CompileTimesMockInterpreterParser : IExpressionParser
    {
        public Dictionary<string, int> ParseCallsByExpressions { get; } = new Dictionary<string, int>();
        public int GlobalCalls { get; private set; }
        public List<Lambda> CreatedLambdas { get; } = new List<Lambda>();

        public Lambda Parse(string expressionText, params Parameter[] parameters)
        {
            if (ParseCallsByExpressions.ContainsKey(expressionText))
                ParseCallsByExpressions[expressionText]++;
            else
                ParseCallsByExpressions.Add(expressionText, 1);

            GlobalCalls++;

            var newLambda = _interpreter.Parse(expressionText, parameters);
            CreatedLambdas.Add(newLambda);

            return newLambda;
        }

        public void SetReference(IEnumerable<ReferenceType> referencedTypes)
        {
            _interpreter.Reference(referencedTypes);
        }

        private Interpreter _interpreter = new Interpreter();
    }
}
