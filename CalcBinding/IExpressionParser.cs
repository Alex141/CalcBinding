using DynamicExpresso;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalcBinding
{
    public interface IExpressionParser
    {
        Lambda Parse(string expressionText, params Parameter[] parameters);
    }

    public class InterpreterParser:IExpressionParser
    {
        Interpreter interpreter;

        public InterpreterParser()
        {
            interpreter = new Interpreter();
        }

        public Lambda Parse(string expressionText, params Parameter[] parameters)
        {
            return interpreter.Parse(expressionText, parameters);
        }
    }

    public class CompileTimesMockInterpreterParser: IExpressionParser
    {
        Interpreter interpreter;
        Dictionary<string, int> parseCalls;

        public CompileTimesMockInterpreterParser()
        {
            interpreter = new Interpreter();        
            parseCalls = new Dictionary<string, int>();
        }

        public Lambda Parse(string expressionText, params Parameter[] parameters)
        {
            if (parseCalls.ContainsKey(expressionText))
                parseCalls[expressionText]++;
            else
                parseCalls.Add(expressionText, 1);

            return interpreter.Parse(expressionText, parameters);
        }
    }
}
