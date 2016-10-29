using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WpfExample;
using System.Collections.Generic;
using System.Windows.Media;

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

        //test on bool to visibility + ternary + color

        // test for error when set binding to static property and source automatically??

        //test: visibility binds to static property bool (bug with (n*{1})n* recognition)

        //readonly static property

        //static property without event (such as Brush) (peharps binding with flag oneWay)

        //mixed test: Math + Char + String + Static + NonStatic + Enum
    }
}
