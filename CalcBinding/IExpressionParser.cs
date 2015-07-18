using DynamicExpresso;

namespace CalcBinding
{
    public interface IExpressionParser
    {
        Lambda Parse(string expressionText, params Parameter[] parameters);
    }
}
