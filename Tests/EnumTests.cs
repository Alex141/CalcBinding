using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WpfExample;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Tests
{
#if NETCOREAPP3_0
    [Microsoft.VisualStudio.TestTools.UnitTesting.STAExtensions.STATestClass]
#else 
    [TestClass]
#endif
    public class EnumTests:CalcBindingSystemTests
    {
        [TestMethod]
        public void BindingToEnumAndConstantTest()
        {
            // Usual + constant
            var exampleViewModel = new ExampleViewModel();

            StringAndObjectBindingAssert("EnumValue == local:Enum2.Value1 ?4:5", exampleViewModel,
                () => exampleViewModel.EnumValue = Enum2.Value1, "4", 4,
                () => exampleViewModel.EnumValue = Enum2.Value2, "5", 5, new Dictionary<string, Type>
                {
                    {"local:Enum2", typeof(Enum2)}
                }
            );

            // Static + constant
            StringAndObjectBindingAssert("local:StaticExampleClass.EnumValue == local:Enum2.Value1 ?4:5", null,
                () => StaticExampleClass.EnumValue = Enum2.Value1, "4", 4,
                () => StaticExampleClass.EnumValue = Enum2.Value2, "5", 5, new Dictionary<string, Type>
                {
                    {"local:StaticExampleClass", typeof(StaticExampleClass)},
                    {"local:Enum2", typeof(Enum2)}
                }
            );
        }

        [TestMethod]
        public void ReturnEnumTest()
        {
            // VisibilityBinding + static
            StaticExampleClass.Visibility = Visibility.Hidden; 
            VisibilityBindingAssert("local:StaticExampleClass.EnumValue == local:Enum2.Value1 ? win:Visibility.Visible : local:StaticExampleClass.Visibility", null,
                () => StaticExampleClass.EnumValue = Enum2.Value1, Visibility.Visible,
                () => StaticExampleClass.EnumValue = Enum2.Value2, Visibility.Hidden, new Dictionary<string, Type>
                {
                    {"local:StaticExampleClass", typeof(StaticExampleClass)},
                    {"win:Visibility", typeof(Visibility)},
                    {"local:Enum2", typeof(Enum2)}
                }
            );

            // ObjectAndStringBinding + usual
            var exampleViewModel = new ExampleViewModel();
            exampleViewModel.Visibility = Visibility.Hidden;
            StringAndObjectBindingAssert("EnumValue == local:Enum2.Value1 ? win:Visibility.Visible : Visibility", exampleViewModel,
                () => exampleViewModel.EnumValue = Enum2.Value1, "Visible", Visibility.Visible,
                () => exampleViewModel.EnumValue = Enum2.Value2, "Hidden", Visibility.Hidden, new Dictionary<string, Type>
                {
                    {"win:Visibility", typeof(Visibility)},
                    {"local:Enum2", typeof(Enum2)}
                }
            );
        }

        [TestMethod]
        public void EnumConstantTest()
        {
            // Visibility + usual
            var exampleViewModel = new ExampleViewModel();
            StringAndObjectBindingAssert("Visibility==win:Visibility.Visible?1:2", exampleViewModel,
                () => exampleViewModel.Visibility = Visibility.Collapsed, "2", 2,
                () => exampleViewModel.Visibility = Visibility.Visible, "1", 1, new Dictionary<string, Type>
                {
                    {"win:Visibility", typeof(Visibility)}
                }
            );

            // Visibility + static
            StringAndObjectBindingAssert("local:StaticExampleClass.Visibility==win:Visibility.Visible?1:2", null,
                () => StaticExampleClass.Visibility = Visibility.Collapsed, "2", 2,
                () => StaticExampleClass.Visibility = Visibility.Visible, "1", 1, new Dictionary<string, Type>
                {
                    {"local:StaticExampleClass", typeof(StaticExampleClass)},
                    {"win:Visibility", typeof(Visibility)}
                }
            );
        }

        [TestMethod]
        public void EnumWithTernaryOperatorTest()
        {
            // right
            ObjectBindingAssert("local:StaticExampleClass.EnumValue == local:Enum2.Value1 ? local:Enum1.Value1 : local:Enum1.Value2", null,
                () => StaticExampleClass.EnumValue = Enum2.Value1, Enum1.Value1,
                () => StaticExampleClass.EnumValue = Enum2.Value2, Enum1.Value2, new Dictionary<string, Type>
                {
                    {"local:StaticExampleClass", typeof(StaticExampleClass)},
                    {"local:Enum1", typeof(Enum1)},
                    {"local:Enum2", typeof(Enum2)}
                }
            );

            // wrong
            ObjectBindingAssert("local:StaticExampleClass.EnumValue == local:Enum2.Value1 ? local:Enum1.Value1:local:Enum1.Value2", null,
                () => StaticExampleClass.EnumValue = Enum2.Value1, null,
                () => StaticExampleClass.EnumValue = Enum2.Value2, null, new Dictionary<string, Type>
                {
                    {"local:StaticExampleClass", typeof(StaticExampleClass)},
                    {"local:Enum1", typeof(Enum1)},
                    {"local:Enum2", typeof(Enum2)}
                }
            );
        }

        [TestMethod]
        public void BindingToSingleEnumTest()
        {
            // Usual
            var exampleViewModel = new ExampleViewModel();

            StringAndObjectBindingAssert("EnumValue", exampleViewModel,
                () => exampleViewModel.EnumValue = Enum2.Value1, "Value1", Enum2.Value1,
                () => exampleViewModel.EnumValue = Enum2.Value2, "Value2", Enum2.Value2
            );

            // Static
            StringAndObjectBindingAssert("local:StaticExampleClass.EnumValue", null,
                () => StaticExampleClass.EnumValue = Enum2.Value1, "Value1", Enum2.Value1,
                () => StaticExampleClass.EnumValue = Enum2.Value2, "Value2", Enum2.Value2, new Dictionary<string, Type>
                {
                    {"local:StaticExampleClass", typeof(StaticExampleClass)},
                }
            );
        }

        [TestMethod]
        public void MultipleEnumBindingsTest()
        {
            var exampleViewModel = new ExampleViewModel();
            var chb1 = new CheckBox();
            var chb2 = new CheckBox();

            var inputs = new List<BindingTestInput>() 
            {
                new BindingTestInput("EnumValue == local:Enum2.Value1", chb1, CheckBox.IsCheckedProperty),
                new BindingTestInput("Enum3Value == local:Enum3.Value11", chb2, CheckBox.IsCheckedProperty),
            };

            var outputs = new List<BindingTestCase<bool?>>()
            {
                new BindingTestCase<bool?>(() => exampleViewModel.EnumValue = Enum2.Value1, () => chb1.IsChecked, true),
                new BindingTestCase<bool?>(() => exampleViewModel.EnumValue = Enum2.Value2, () => chb1.IsChecked, false),
                new BindingTestCase<bool?>(() => exampleViewModel.Enum3Value = Enum3.Value11, () => chb2.IsChecked, true),
                new BindingTestCase<bool?>(() => exampleViewModel.Enum3Value = Enum3.Value22, () => chb2.IsChecked, false),
            };

            var resolvedTypes = new Dictionary<string, Type>()
            {
                { "local:Enum2", typeof(Enum2) },
                { "local:Enum3", typeof(Enum3) }
            };

            MulitpleBindingAssert(inputs, exampleViewModel, outputs, resolvedTypes);
        }
    }
}
