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
            testInverse("a + 2", "((Path) - 2)");
            testInverse("4 + a", "((Path) - 4)");
            //+
            testInverse("a + 2 + 3 + 5 + 2", "(((((Path) - 2) - 5) - 3) - 2)");
            //-
            testInverse("a - 5", "((Path) + 5)");
            testInverse("4 - a", "(4 - (Path))");
               
            //*
            testInverse("a * 3", "((Path) / 3)");
            testInverse("3 * a", "((Path) / 3)");

            // /
            testInverse("a / 7", "((Path) * 7)");
            testInverse("3 / a", "(3 / (Path))"); 

            //complex
            testInverse("((a + 2 - 5 * 3) / (2 - 7) - 3) * 2 / 3", "(((((((Path)*3)/2)+3)*(2 - 7))+(5 * 3))-2)");

            testInverse("(int)((double)5.0 / (17-a))", "(17-(5/(Path)))");
        }

        [TestMethod]
        [ExpectedException(typeof(InverseException))]
        public void ConstantExpressionTest()
        {
            //constant complex
            testInverse("2 / 5 * 4 + 5 - (6 + 3) * 2 / 4", "");
        }

        [TestMethod]
        public void BadExpressionsTest()
        {
            AssertException<InverseException>(() => testInverse("a % 2", ""));
            AssertException<InverseException>(() => testInverse("(int)Math.Max(a, 4)", ""));
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

        private void testInverse(string expr, string exceptedResult)
        {
            var inverse = new Inverter();
            var interpreter = new Interpreter();

            var aParam = new Parameter("a", typeof(int));
            var resParam = Expression.Parameter(typeof(double), "Path");
            var resParam2 = new Parameter("Path", typeof(double));

            var realInverseExpr = inverse.InverseExpression(interpreter.Parse(expr, aParam).Expression, resParam).ToString();
            var expectedInverseExpr = interpreter.Parse(exceptedResult, resParam2).Expression.ToString();
            Assert.AreEqual(expectedInverseExpr, realInverseExpr);
        }
    }
}
