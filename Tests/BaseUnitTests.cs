using CalcBinding;
using CalcBinding.ExpressionParsers;

namespace Tests
{
    public abstract class BaseUnitTests
    {
        protected IExpressionParser CreateParser(IExpressionParser innerParser = null)
        {
            var parserFactory = new ParserFactory();
            return parserFactory.CreateCachedParser(innerParser);
        }

        protected CalcConverter CreateConverter(IExpressionParser innerParser = null)
        {
            return new CalcConverter(CreateParser(innerParser), null, null);
        }
    }
}
