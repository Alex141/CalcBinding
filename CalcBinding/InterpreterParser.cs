using DynamicExpresso;

namespace CalcBinding
{
    public class InterpreterParser: IExpressionParser
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