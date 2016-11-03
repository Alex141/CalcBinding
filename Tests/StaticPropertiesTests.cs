using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WpfExample;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows;
using System.Globalization;

namespace Tests
{
    [TestClass]
    public class StaticPropertiesTests:BaseSystemTests
    {
        [TestMethod]
        public void SingleStaticPropertyWithBracketsTest()
        {  
            StringAndObjectBindingAssert("(local:StaticExampleClass.StaticA)", null,
                () => StaticExampleClass.StaticA = 10, "10", (double)10,
                () => StaticExampleClass.StaticA = 20.34, "20.34", 20.34,
                new Dictionary<string, Type>() { {"local:StaticExampleClass", typeof(StaticExampleClass) }}
            );

            var binding = new CalcBinding.Binding();
            binding.Path = "(local:StaticExampleClass.StaticA)";

            StringAndObjectBindingAssert(binding, null,
                () => StaticExampleClass.StaticA = 10, "10", (double)10,
                () => StaticExampleClass.StaticA = 20.34, "20.34", 20.34,
                new Dictionary<string, Type>() { { "local:StaticExampleClass", typeof(StaticExampleClass) } }
            );
        }

        [TestMethod]
        public void SingleStaticPropertyWithoutBracketsTest()
        {
            StringAndObjectBindingAssert("local:StaticExampleClass.StaticA", null,
                () => StaticExampleClass.StaticA = 10, "10", (double)10,
                () => StaticExampleClass.StaticA = 20.34, "20.34", 20.34,
                new Dictionary<string, Type>() { { "local:StaticExampleClass", typeof(StaticExampleClass) } }
            );

            var binding = new CalcBinding.Binding();
            binding.Path = "local:StaticExampleClass.StaticA";

            StringAndObjectBindingAssert(binding, null,
                () => StaticExampleClass.StaticA = 10, "10", (double)10,
                () => StaticExampleClass.StaticA = 20.34, "20.34", 20.34,
                new Dictionary<string, Type>() { { "local:StaticExampleClass", typeof(StaticExampleClass) } }
            );
        }

        [TestMethod]
        public void BindingToStaticClassTest()
        {
            StringAndObjectBindingAssert("local:StaticStaticClass.StaticA", null,
                () => StaticStaticClass.StaticA = 10, "10", (double)10,
                () => StaticStaticClass.StaticA = 20.34, "20.34", 20.34,
                new Dictionary<string, Type>() { { "local:StaticStaticClass", typeof(StaticStaticClass) } }
            );
        }

        [TestMethod]
        public void AlgebraicStaticPropertyTest()
        {
            StringAndObjectBindingAssert("local:StaticExampleClass.StaticA+5", null,
                () => StaticExampleClass.StaticA = 10, "15", (double)15,
                () => StaticExampleClass.StaticA = 20.34, "25.34", 25.34,
                new Dictionary<string, Type>() { { "local:StaticExampleClass", typeof(StaticExampleClass) } }
            );

            StringAndObjectBindingAssert("local:StaticExampleClass.StaticA+local:StaticExampleClass.StaticB", null,
                () => { StaticExampleClass.StaticA = 10.4; StaticExampleClass.StaticB = 2; }, "12.4", (double)12.4,
                () => {StaticExampleClass.StaticA = 20.5; StaticExampleClass.StaticB = -20;}, "0.5", (double)0.5,
                new Dictionary<string, Type>() { { "local:StaticExampleClass", typeof(StaticExampleClass) } }
            );

            var exampleViewModel = new ExampleViewModel();
            
            StringAndObjectBindingAssert("local:StaticExampleClass.StaticA-A", exampleViewModel,
                () => { StaticExampleClass.StaticA = 10;    exampleViewModel.A = -4; }, "14", (double)14,
                () => { StaticExampleClass.StaticA = 20.34; exampleViewModel.A = 8; }, "12.34", 12.34,
                    new Dictionary<string, Type>() { { "local:StaticExampleClass", typeof(StaticExampleClass) } }
            );
        }

        [TestMethod]
        public void ReadonlyPropertiesTest()
        {
            var exampleViewModel = new ExampleViewModel();

            BrushBindingAssert("(A>B) ? m:Brushes.White : m:Brushes.Black", exampleViewModel,
                () => { exampleViewModel.A = 10; exampleViewModel.B = 15;}, Brushes.Black,
                () => { exampleViewModel.A = 20; exampleViewModel.B = -2; }, Brushes.White,
                new Dictionary<string, Type>() { { "m:Brushes", typeof(Brushes) } }
            );
        }

        [TestMethod]
        public void TernaryOperatorWithStringTest()
        {
            var exampleViewModel = new ExampleViewModel();

            StringAndObjectBindingAssert("(A < B?'LightBlue when A < B':'Red when A >= B') ", exampleViewModel,
                () => { exampleViewModel.A = 10; exampleViewModel.B = 15; }, "LightBlue when A < B", "LightBlue when A < B",
                () => { exampleViewModel.A = 20; exampleViewModel.B = -2; }, "Red when A >= B", "Red when A >= B"
            );
        }

        [TestMethod]
        public void TwoStaticPropertyPathesTest()
        {
            BrushBindingAssert("(local:StaticExampleClass.StaticA > 5?m:Brushes.White : m:Brushes.Black) ", null,
                () => { StaticExampleClass.StaticA = 4; }, Brushes.Black,
                () => { StaticExampleClass.StaticA = 10; }, Brushes.White,
                new Dictionary<string, Type> 
                { 
                    {"local:StaticExampleClass", typeof(StaticExampleClass)},
                    {"m:Brushes", typeof(Brushes)}
                }
            );
        }

        [TestMethod]
        public void BoolToVisibilityTest()
        {
            VisibilityBindingAssert("local:StaticExampleClass.StaticA less 5", null,
                () => { StaticExampleClass.StaticA = 4; }, Visibility.Visible,
                () => { StaticExampleClass.StaticA = 10; }, Visibility.Collapsed,
                new Dictionary<string, Type> 
                { 
                    {"local:StaticExampleClass", typeof(StaticExampleClass)},
                }
            );
        }

        [TestMethod]
        public void UpdateSourceStaticPropertyTest()
        {
            StringAndObjectBindingBackAssert("local:StaticExampleClass.StaticA+5", null, () => StaticExampleClass.StaticA,
                "10", "-2", 10, -2,
                5d, -7d, new Dictionary<string, Type>
                {
                    {"local:StaticExampleClass", typeof(StaticExampleClass)}
                });
        }

        [TestMethod]
        public void MathWithStaticPropertyTest()
        {
            StringAndObjectBindingAssert("Math.Round(Math.Sin(local:StaticExampleClass.StaticB *Math.PI / 2.0))", null, 
                () => { StaticExampleClass.StaticB = 1; }, "1", 1d,
                () => { StaticExampleClass.StaticB = 6; }, "0", 0d,
                new Dictionary<string, Type>
                {
                    {"local:StaticExampleClass", typeof(StaticExampleClass)}
                });

            StringAndObjectBindingAssert("Math.Round(Math.Cos(local:StaticExampleClass.StaticB *Math.PI / 2.0))", null,
                () => { StaticExampleClass.StaticB = 3; }, "0", 0d,
                () => { StaticExampleClass.StaticB = 4; }, "1", 1d,
                new Dictionary<string, Type>
                {
                    {"local:StaticExampleClass", typeof(StaticExampleClass)}
                });
        }

        [TestMethod]
        public void MathTwoWayWithStaticPropertyTest()
        {
            StringAndObjectBindingBackAssert("Math.Sin(local:StaticExampleClass.StaticB/100.0)", null, () => StaticExampleClass.StaticB,
                "0.5", "-0.9", 0.5, -0.9,
                52, -111, new Dictionary<string, Type>
                {
                    {"local:StaticExampleClass", typeof(StaticExampleClass)}
                });

            StringAndObjectBindingBackAssert("Math.Cos(local:StaticExampleClass.StaticB/100.0)", null, () => StaticExampleClass.StaticB,
                "0.5", "-0.9", 0.5, -0.9,
                104, 269, new Dictionary<string, Type>
                {
                    {"local:StaticExampleClass", typeof(StaticExampleClass)}
                });

            //PI

            var pi = Math.PI.ToString(CultureInfo.InvariantCulture);
            StringAndObjectBindingBackAssert("Math.Pow(Math.PI,local:StaticExampleClass.StaticA)", null, () => StaticExampleClass.StaticA,
                pi, "1", Double.Parse(pi, CultureInfo.InvariantCulture), 1,
                0.999999999999999, 0.0, new Dictionary<string, Type>
                {
                    {"local:StaticExampleClass", typeof(StaticExampleClass)}
                });
        }

        [TestMethod]
        public void DefaultDifferQuotesIsFalseTest()
        {
            // default - DifferQuotes = false
            var binding = new CalcBinding.Binding("A");
            Assert.AreEqual(false, binding.DifferQuotes);

            var exampleViewModel = new ExampleViewModel();

            StringBindingAssert("Name + 'D'", exampleViewModel,
                () => exampleViewModel.Name = "ABC", "ABCD",
                () => exampleViewModel.Name = "A", "AD"
            );

            StringBindingAssert("Name + \"DEF\"", exampleViewModel,
                () => exampleViewModel.Name = "ABC", "ABCDEF",
                () => exampleViewModel.Name = "A", "ADEF"
            );
        }

        [TestMethod]
        public void DifferQuotesSettedToTrueTest()
        {
            var binding = new CalcBinding.Binding("Name + \"DEF\"");
            binding.DifferQuotes = true;

            var exampleViewModel = new ExampleViewModel();
            StringBindingAssert(binding, exampleViewModel,
                () => exampleViewModel.Name = "ABC", "ABCDEF",
                () => exampleViewModel.Name = "A", "ADEF"
            );

            //detect char
            binding = new CalcBinding.Binding("Name + 'D'");
            binding.DifferQuotes = true;

            StringBindingAssert(binding, exampleViewModel,
                () => exampleViewModel.Name = "ABC", "",
                () => exampleViewModel.Name = "A", ""
            );
        }

        [TestMethod]
        public void SingleQuotesInStringTest()
        {
            //detect single quotes
            var binding = new CalcBinding.Binding("l:StaticExampleClass2.Name + \"'D'EF\"");
            binding.DifferQuotes = true;

            StringBindingAssert(binding, null,
                () => StaticExampleClass.Name = "ABC", "ABC'D'EF",
                () => StaticExampleClass.Name = "A", "A'D'EF", new Dictionary<string, Type>
                {
                    {"l:StaticExampleClass2", typeof(StaticExampleClass)}
                }
            );

            binding = new CalcBinding.Binding("Name + \"D'EF\"");
            binding.DifferQuotes = true;

            var exampleViewModel = new ExampleViewModel();
            StringBindingAssert(binding, exampleViewModel,
                () => exampleViewModel.Name = "ABC", "ABCD'EF",
                () => exampleViewModel.Name = "A", "AD'EF"
            );
        }

        [TestMethod]
        public void DoubleQuotesInCharTest()
        {
            var binding = new CalcBinding.Binding("local:StaticExampleClass.StaticChar == 'A' ? '4' : '\"'");
            binding.DifferQuotes = true;

            StringAndObjectBindingAssert(binding, null,
                () => StaticExampleClass.StaticChar = 'D', "\"", '"',
                () => StaticExampleClass.StaticChar = 'A', "4", '4', new Dictionary<string, Type>
                {
                    {"local:StaticExampleClass", typeof(StaticExampleClass)}
                }
            );

        }

        [TestMethod]
        public void BindingToStaticPropertyThatRaisePersonalEventTest()
        {
            StringAndObjectBindingAssert("local:StaticExampleClass.StaticAWithPersonalEvent", null,
                () => StaticExampleClass.StaticAWithPersonalEvent = 10, "10", (double)10,
                () => StaticExampleClass.StaticAWithPersonalEvent = 20.34, "20.34", 20.34,
                new Dictionary<string, Type>() { { "local:StaticExampleClass", typeof(StaticExampleClass) } }
            );           
        }
        
        // test for error when set binding to static property and source automatically??

        //test: visibility binds to static property bool (bug with (n*{1})n* recognition)

        //readonly static property

        //static property without event (such as Brush) (peharps binding with flag oneWay)

        //mixed test: Math + Char + String + Static + NonStatic + Enum
    }
}
