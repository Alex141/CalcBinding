using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CalcBinding.Inverse;
using System.Linq.Expressions;
using DynamicExpresso;
using System.Collections.Generic;

namespace Tests
{
    [TestClass]
    public class InverseTest
    {
        [TestMethod]
        public void BasicExpressionsTest()
        {
            Action<string, string> basicTestInverse = 
                (expr, exceptedResult) => testInverse<int, double>(expr, exceptedResult);

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

            basicTestInverse("(int)((double)5.0 / (17-a))", "(17-(5/(Path)))");
        }

        [TestMethod]
        [ExpectedException(typeof(InverseException))]
        public void ConstantExpressionTest()
        {
            //constant complex
            testInverse<int, double>("2 / 5 * 4 + 5 - (6 + 3) * 2 / 4", "");
        }

        [TestMethod]
        public void BadExpressionsTest()
        {
            AssertException<InverseException>(() => testInverse<int, double>("a % 2", ""));
            AssertException<InverseException>(() => testInverse<int, double>("(int)Math.Max(a, 4)", ""));
        }

        [TestMethod]
        public void MathExpressionsTest()
        {
            testDoubleInverse<double, double>("Math.Sin(a)", "Math.Asin(Path)");
            testDoubleInverse<double, double>("Math.Cos(a)", "Math.Acos(Path)");
            testDoubleInverse<double, double>("Math.Pow(a, 2)", "Math.Pow(Path, 1/2)");
            testInverse<double, double>("Math.Pow(4, a)", "Math.Log(Path, 4)");
            testInverse<double, double>("Math.Log(a, 4)", "Math.Pow(4, Path)");
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

        private void testDoubleInverse<aType, PathType>(string expr, string exceptedResult)
        {
            testInverse<aType, PathType>(expr, exceptedResult);
            testInverse<PathType, aType>(exceptedResult, expr, true);
        }

        private void testInverse<aType, PathType>(string expr, string exceptedResult, bool reverse = false)
        {
            var inverse = new Inverter();
            var interpreter = new Interpreter();

            String argName = "a", resName = "Path";

            if (reverse)
            {
                swap(ref argName, ref resName);
                //swap(ref expr, ref exceptedResult);
            }

            var aParam = new Parameter(argName, typeof(aType));
            var resParam = Expression.Parameter(typeof(PathType), resName);
            var resParam2 = new Parameter(resName, typeof(PathType));

            var realInverseExpr = inverse.InverseExpression(interpreter.Parse(expr, aParam).Expression, resParam).Expression.ToString();
            var expectedInverseExpr = interpreter.Parse(exceptedResult, resParam2).Expression.ToString();
            Assert.AreEqual(expectedInverseExpr, realInverseExpr);
        }

        private void swap<T>(ref T arg1, ref T arg2)
        {
            T temp = arg1;

            arg1 = arg2;

            arg2 = temp;
        }
    }
}
