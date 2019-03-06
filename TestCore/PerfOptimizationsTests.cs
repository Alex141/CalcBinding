using DynamicExpresso;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Tests.Mocks;
using WpfExample;

namespace Tests
{
    [TestClass]
    public sealed class PerfOptimizationsTests : BaseSystemTests
    {
        [Fact]
        public void OneExpressionCompilesOneTimeTest()
        {
            var compileTimesParser = new CompileTimesMockInterpreterParser();

            try
            {
                CalcBinding.Binding.ReplaceExpressionParser(CreateParser(compileTimesParser));

                var test = new ExampleViewModel();

                Assert.AreEqual(0, compileTimesParser.GlobalCalls);

                for (var i = 0; i < 10; i++)
                {
                    StringAndObjectBindingAssert("A-B", test,
                        () => { test.A = 20; test.B = 10; }, "10", (double)10,
                        () => { test.A = 20.34; test.B = 15; }, "5.34", 5.34
                    );
                }

                Assert.AreEqual(1, compileTimesParser.GlobalCalls);

                StringAndObjectBindingAssert("A-C", test,
                    () => { test.A = 10; test.C = -2; }, "12", (double)12,
                    () => { test.A = 20.34; test.C = 12; }, "8.34", 8.34
                );

                Assert.AreEqual(1, compileTimesParser.GlobalCalls);

                StringAndObjectBindingAssert("C-A", test,
                    () => { test.A = 10; test.C = -2; }, "-12", (double)-12,
                    () => { test.A = 20.34; test.C = 52; }, "31.66", 31.66
                );

                Assert.AreEqual(2, compileTimesParser.GlobalCalls);
            }
            finally
            {
                CalcBinding.Binding.ReplaceExpressionParser(CreateParser());
            }
        }

        [Fact]
        public void OneBackExpressionCompilesOneTimeTest()
        {
            var compileTimesParser = new CompileTimesMockInterpreterParser();

            try
            {
                CalcBinding.Binding.ReplaceExpressionParser(CreateParser(compileTimesParser));

                var test = new ExampleViewModel();

                Assert.AreEqual(0, compileTimesParser.GlobalCalls);

                for (var i = 0; i < 10; i++)
                {
                    StringAndObjectBindingBackAssert("B+5", test, () => test.B,
                                    "10", "-5", 10, -5,
                                    5, -10);
                }

                Assert.AreEqual(2, compileTimesParser.GlobalCalls);

                StringAndObjectBindingBackAssert("C+5", test, () => test.C,
                                "100", "-50", 100, -50,
                                95, -55);

                Assert.AreEqual(2, compileTimesParser.GlobalCalls);

                StringAndObjectBindingBackAssert("C+6", test, () => test.C,
                                "100", "-50", 100, -50,
                                94, -56);

                Assert.AreEqual(4, compileTimesParser.GlobalCalls);
            }
            finally
            {
                CalcBinding.Binding.ReplaceExpressionParser(CreateParser());
            }
        }

        [Fact]
        public void NotUsedExpressionsMustBeCollected()
        {
            var compileTimesParser = new CompileTimesMockInterpreterParser();

            try
            {
                CalcBinding.Binding.ReplaceExpressionParser(CreateParser(compileTimesParser));

                var test = new ExampleViewModel();

                Assert.AreEqual(0, compileTimesParser.GlobalCalls);

                StringAndObjectBindingBackAssert("C+5", test, () => test.C,
                                "100", "-50", 100, -50,
                                95, -55);

                Assert.AreEqual(2, compileTimesParser.GlobalCalls);

                var lambdasWeakRefs = GetLambdasWeakRefs(compileTimesParser, 2);
                compileTimesParser.CreatedLambdas.Clear();

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                Assert.IsTrue(lambdasWeakRefs.All(r => !r.IsAlive), "cached lambdas should be collected");

                StringAndObjectBindingBackAssert("C+5", test, () => test.C,
                                "100", "-50", 100, -50,
                                95, -55);

                Assert.AreEqual(4, compileTimesParser.GlobalCalls);
            }
            finally
            {
                CalcBinding.Binding.ReplaceExpressionParser(CreateParser());
            }
        }

        private List<WeakReference> GetLambdasWeakRefs(CompileTimesMockInterpreterParser compileTimesParser, int expectedLambdasCount)
        {
            Assert.AreEqual(expectedLambdasCount, compileTimesParser.CreatedLambdas.Count);

            return compileTimesParser
                    .CreatedLambdas
                    .Select(l => new WeakReference(l))
                    .ToList();
        }
    }
}
