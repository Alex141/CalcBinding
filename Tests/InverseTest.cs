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
            #region примеры
            var exprList = new Dictionary<Expression<Func<int, double>>, Expression<Func<double, double>>>() 
            {
                //+
                { (a) => a + 2, (Path) => Path - 2 },
                { (a) => 4 + a, (Path) => Path - 4 },
                { (a) => a + 2 + 3 + 5 + 2, (Path) => Path - 2 - 5 - 3 - 2 },
                //-
                { (a) => a - 5, (Path) => Path + 5 },
                { (a) => 4 - a, (Path) => 4 - Path }
               
                ////*
                //(a) => a * 3,
                //(a) => 3 * a,

                //// /
                //(a) => a / 7,
                //(a) => 3 / a,

                ////complex
                //(a) => ((a + 2 - 5 * 3) / (2 - 7) - 3) * 2 / 3,

                //(a) => (int)((double)5.0 / (17-a)),
 
                ////constant complex
                //(a) => 2 / 5 * 4 + 5 - (6 + 3) * 2 / 4,

                ////bad samples
                //(a) => a % 2,
                //(a) => (int)Math.Max(a, 4)
            };
            #endregion


            var inverse = new Inverter();
            var interpreter = new Interpreter();

            var resParam = Expression.Parameter(typeof(double), "Path");
            var resParam2 = new Parameter("Path", typeof(double));

            foreach (var expr in exprList.Keys)
            {
                var expectedInverseExpr = interpreter.Parse(exprList[expr].Body.ToString(), resParam2).Expression.ToString();
                Assert.AreEqual(expectedInverseExpr, inverse.InverseExpression(expr.Body, resParam).ToString());
            }
        }
    }
}
