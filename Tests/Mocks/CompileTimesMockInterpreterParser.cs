using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalcBinding;
using DynamicExpresso;

namespace Tests.Mocks
{
    public class CompileTimesMockInterpreterParser : IExpressionParser
    {
        Interpreter interpreter;

        public Dictionary<string, int> ParseCalls { get; private set; }

        public CompileTimesMockInterpreterParser()
        {
            interpreter = new Interpreter();
            ParseCalls = new Dictionary<string, int>();
        }

        public Lambda Parse(string expressionText, params Parameter[] parameters)
        {
            if (ParseCalls.ContainsKey(expressionText))
                ParseCalls[expressionText]++;
            else
                ParseCalls.Add(expressionText, 1);

            return interpreter.Parse(expressionText, parameters);
        }
    }
}
