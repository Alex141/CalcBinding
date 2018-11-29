using System;
using System.Linq.Expressions;
using CalcBinding;
using CalcBinding.ExpressionParsers;
using CalcBinding.Inversion;
using DynamicExpresso;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public sealed class InverseTest : BaseUnitTests
    {
        [TestMethod]
        public void BasicExpressionsTest()
        {
            Action<string, string> basicTestInverse = 
                (expr, exceptedResult) => TestInverse<int, double>(expr, exceptedResult);

            basicTestInverse("a + 2", "((Path) - 2)");
            basicTestInverse("4 + a", "((Path) - 4)");
            //+
            basicTestInverse("a + 2 + 3 + 5 + 2", "(((((Path) - 2) - 5) - 3) - 2)");
            //-
            basicTestInverse("a - 5", "((Path) + 5)");
            basicTestInverse("4 - a", "(4 - (Path))");
               
            //*
            basicTestInverse("a * 3", "((Path) / 3)");
            basicTestInverse("3 * a", "((Path) / 3)");

            // /
            basicTestInverse("a / 7", "((Path) * 7)");
            basicTestInverse("3 / a", "(3 / (Path))"); 

            //complex
            basicTestInverse("((a + 2 - 5 * 3) / (2 - 7) - 3) * 2 / 3", "(((((((Path)*3)/2)+3)*(2 - 7))+(5 * 3))-2)");

            basicTestInverse("(int)((double)5.0 / (17-a))", "((17)-(((Int32)((5)/(((Double)(((Double)(Path)))))))))");

            //yeeeeeeeeeeeah!! gggoodd!!))
            basicTestInverse("(int)(5.31*a)", "(((Int32)((((Double)(Path)))/(5.31))))");
        }

        [TestMethod]
        [ExpectedException(typeof(InverseException))]
        public void ConstantExpressionTest()
        {
            //constant complex
            TestInverse<int, double>("2 / 5 * 4 + 5 - (6 + 3) * 2 / 4", "");
        }

        [TestMethod]
        public void BadExpressionsTest()
        {
            AssertException<InverseException>(() => TestInverse<int, double>("a % 2", ""));
            AssertException<InverseException>(() => TestInverse<int, double>("(int)Math.Max(a, 4)", ""));
        }

        [TestMethod]
        public void MathExpressionsTest()
        {
            TestDoubleInverse<double, double>("Math.Sin(a)", "Math.Asin(Path)");
            TestDoubleInverse<double, double>("Math.Cos(a)", "Math.Acos(Path)");
            TestInverse<double, double>("Math.Tan(a)", "Math.Atan(Path)");
            TestDoubleInverse<double, double>("Math.Pow(4, a)", "Math.Log(Path, 4)");
            TestInverse<double, double>("Math.Pow(a, 2)", "Math.Pow((Path), 1.0/((Double)(2)))");
            TestInverse<double, double>("Math.Pow((a), 1.0/((Double)(2)))", "(Math.Pow((Path), 1.0/((1)/((Double)(2)))))");
            TestInverse<double, double>("Math.Sin((a+5)*Math.PI/4.0)", "((((Math.Asin(Path))*(4))/(Math.PI))-((Double)(5)))");
        }

        private void AssertException<T>(Action action) where T: Exception
        {
            try
            {
                action();
            }
            catch (T)
            {
                return;
            }
            Assert.Fail();
        }

        private void TestDoubleInverse<aType, PathType>(string expr, string exceptedResult)
        {
            TestInverse<aType, PathType>(expr, exceptedResult);
            TestInverse<PathType, aType>(exceptedResult, expr, true);
        }

        private void TestInverse<aType, PathType>(string expr, string exceptedResult, bool reverse = false)
        {
            var inverse = new Inverter(CreateParser());
            var interpreter = new Interpreter();

            String argName = "a", resName = "Path";

            if (reverse)
            {
                Swap(ref argName, ref resName);
                //swap(ref expr, ref exceptedResult);
            }

            var aParam = new Parameter(argName, typeof(aType));
            var resParam = Expression.Parameter(typeof(PathType), resName);
            var resParam2 = new Parameter(resName, typeof(PathType));

            var realInverseExpr = inverse.InverseExpression(interpreter.Parse(expr, aParam).Expression, resParam).Expression.ToString();
            var expectedInverseExpr = interpreter.Parse(exceptedResult, resParam2).Expression.ToString();
            Assert.AreEqual(expectedInverseExpr, realInverseExpr);
        }

        private void Swap<T>(ref T arg1, ref T arg2)
        {
            T temp = arg1;

            arg1 = arg2;

            arg2 = temp;
        }
    }
}
