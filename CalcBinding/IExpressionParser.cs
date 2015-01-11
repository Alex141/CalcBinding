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

        public Lambda Parse(string expressionText, Parameter[] parameters)
        {
            return interpreter.Parse(expressionText, parameters);
        }
    }
}
