using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Windows;
using System.Windows.Controls;
using WpfExample;
using Tests.Mocks;
using CalcBinding;
using System.Windows.Data;
using System.Windows.Media;
using System.Globalization;

namespace Tests
{
    /// <summary>
    /// System tests
    /// </summary>

#if NETCOREAPP3_0
    [Microsoft.VisualStudio.TestTools.UnitTesting.STAExtensions.STATestClass]
#else 
    [TestClass]
#endif
    public class CalcBindingSystemTests : BaseSystemTests
    {
        //--------------------Convert-----------------------------------//

        [TestMethod]
        public void MathTest()
        {
            var test = new ExampleViewModel();

            StringAndObjectBindingAssert("A", test,
                () => test.A = 10, "10", (double)10,
                () => test.A = 20.34, "20.34", 20.34
            );

            StringAndObjectBindingAssert("A+B+C", test,
                () => { test.A = 10; test.B = 20; test.C = -2; }, "28", (double)28,
                () => { test.A = 20.34; test.B = 15; test.C = 12; }, "47.34", 47.34
            );

            StringAndObjectBindingAssert("A-B-C", test,
                () => { test.A = 10; test.B = 20; test.C = 5; }, "-15", (double)-15,
                () => { test.A = 5; test.B = 3; test.C = -7; }, "9", (double)9
            );

            StringAndObjectBindingAssert("A*(B-C)", test,
                () => { test.A = 10; test.B = 20; test.C = 5; }, "150", (double)150,
                () => { test.A = 5.4; test.B = 3; test.C = -8; },
                59.400000000000006.ToString(CultureInfo.InvariantCulture),
                59.400000000000006

            );

            StringAndObjectBindingAssert("2*A-B*0.5", test,
                () => { test.A = 10; test.B = 20; }, "10", (double)10,
                () => { test.A = 5.4; test.B = 3; }, "9.3", (double)9.3
            );

            StringAndObjectBindingAssert("A % B", test,
                () => { test.A = 10; test.B = 4; }, "2", (double)2,
                () => { test.A = 12; test.B = 7; }, "5", (double)5
            );

            //String Format with many values
            var calcBinding = new CalcBinding.Binding("A/B")
            {
                StringFormat = "{0:n2}"
            };

            StringBindingAssert(calcBinding, test,
                () => { test.A = 10; test.B = 3; }, "3.33",
                () => { test.A = 20; test.B = -30; }, "-0.67"
            );

            //String Format with one value
            calcBinding = new CalcBinding.Binding("Math.Sin(A)")
            {
                StringFormat = "{0:n2}"
            };

            StringBindingAssert(calcBinding, test,
                () => { test.A = 10; }, "-0.54",
                () => { test.A = 20; }, "0.91"
            );

            //many entries of parameter
            StringAndObjectBindingAssert("A + 0.5*NestedViewModel.A + A + B + C + A - B", test,
                () => { test.A = 10; test.NestedViewModel.A = 4; test.B = 5; test.C = 2; }, "34", (double)34,
                () => { test.A = 12; test.NestedViewModel.A = 14; test.B = -2; test.C = 9; }, "52", (double)52
            );

            StringAndObjectBindingAssert("-4+B", test,
                () => { test.B = 7; }, "3", 3,
                () => { test.B = 12; }, "8", 8
            );
        }

        [TestMethod]
        public void StringTest()
        {
            var test = new ExampleViewModel();

            StringBindingAssert("Name + ' ' + Surname", test,
                () => { test.Name = "Willy"; test.Surname = "White"; }, "Willy White",
                () => { test.Name = ""; test.Surname = "Smith"; }, " Smith"
            );

            StringBindingAssert("(IsMan ? 'Mr' : 'Ms') + ' ' + Name + ' ' + Surname", test,
                () => { test.Name = "Willy"; test.Surname = "White"; test.IsMan = true; }, "Mr Willy White",
                () => { test.Name = "Jane"; test.Surname = "Brown"; test.IsMan = false; }, "Ms Jane Brown"
            );
        }

        [TestMethod]
        public void LogicTest()
        {
            var test = new ExampleViewModel();

            StringAndObjectBindingAssert("(A == B) ? 10 : 20", test,
                () => { test.A = 4; test.B = 4; }, "10", 10,
                () => { test.A = 5; }, "20", 20
            );

            BoolBindingAssert("!IsChecked", test,
                () => test.IsChecked = true, false,
                () => test.IsChecked = false, true
            );

            BoolBindingAssert("not IsChecked", test,
                () => test.IsChecked = true, false,
                () => test.IsChecked = false, true
            );

            BoolBindingAssert("!IsChecked and IsFull", test,
                () => { test.IsChecked = false; test.IsFull = false; }, false,
                () => { test.IsChecked = false; test.IsFull = true; }, true
            );

            BoolBindingAssert("IsChecked and not IsFull", test,
                () => { test.IsChecked = true; test.IsFull = true; }, false,
                () => { test.IsChecked = true; test.IsFull = false; }, true
            );

            BoolBindingAssert("!IsChecked && IsFull", test,
                () => { test.IsChecked = false; test.IsFull = false; }, false,
                () => { test.IsChecked = false; test.IsFull = true; }, true
            );

            BoolBindingAssert("!IsChecked or (A > B)", test,
                () => { test.IsChecked = false; test.A = 20; test.B = 10; }, true,
                () => { test.IsChecked = true; test.A = 4; }, false
            );

            BoolBindingAssert("IsChecked || !IsFull", test,
                () => { test.IsChecked = false; test.IsFull = true; }, false,
                () => { test.IsFull = false; }, true
            );

            BoolBindingAssert("A == 1 and (B <= 5)", test,
                () => { test.A = 1; test.B = 10; }, false,
                () => { test.B = 4; }, true
            );

            BoolBindingAssert("A == 1 and (B less= 5)", test,
                () => { test.A = 1; test.B = 6; }, false,
                () => { test.B = 5; }, true
            );

            BoolBindingAssert("A == 1 and (B less= 5)", test,
                () => { test.A = 1; test.B = 5; }, true,
                () => { test.B = 4; }, true
            );

            BoolBindingAssert("A == 1 and (B less 5)", test,
                () => { test.A = 1; test.B = 6; }, false,
                () => { test.B = 5; }, false
            );

            BoolBindingAssert("A == 1 and (B less 5)", test,
                () => { test.A = 1; test.B = 5; }, false,
                () => { test.B = 4; }, true
            );
        }

        [TestMethod]
        public void VisibilityTest()
        {
            //-------------------------------------------------------------------//
            var test = new ExampleViewModel();

            //-------------------------------------------------------------------//
            VisibilityBindingAssert("!HasPrivileges", test,
                () => { test.HasPrivileges = true; }, Visibility.Collapsed,
                () => { test.HasPrivileges = false; }, Visibility.Visible
            );

            //-------------------------------------------------------------------//
            var calcBinding = new CalcBinding.Binding("!HasPrivileges")
            {
                FalseToVisibility = FalseToVisibility.Collapsed
            };

            VisibilityBindingAssert(calcBinding, test,
                () => { test.HasPrivileges = true; }, Visibility.Collapsed,
                () => { test.HasPrivileges = false; }, Visibility.Visible
            );

            //-------------------------------------------------------------------//
            calcBinding = new CalcBinding.Binding("!HasPrivileges")
            {
                FalseToVisibility = FalseToVisibility.Hidden
            };

            VisibilityBindingAssert(calcBinding, test,
                () => { test.HasPrivileges = true; }, Visibility.Hidden,
                () => { test.HasPrivileges = false; }, Visibility.Visible
            );
            //-------------------------------------------------------------------//
        }

        [TestMethod]
        public void NestedViewModelTest()
        {
            var test = new ExampleViewModel();

            StringAndObjectBindingAssert("A+NestedViewModel.A*0.2+C", test,
                () => { test.A = 10; test.NestedViewModel.A = 30; test.C = -2; }, "14", (double)14,
                () => { test.A = 20.34; test.NestedViewModel.A = 15; test.C = 12; }, "35.34", 35.34
            );

            StringAndObjectBindingAssert("A+NestedViewModel.A*0.2+NestedViewModel.DoubleNestedViewModel.B/8", test,
                () => { test.A = 10; test.NestedViewModel.A = 30; test.NestedViewModel.DoubleNestedViewModel.B = 32; }, "20", (double)20,
                () => { test.A = 20.34; test.NestedViewModel.A = 15; test.NestedViewModel.DoubleNestedViewModel.B = 17; }, "25.34", 25.34
            );

            StringAndObjectBindingAssert("A+NestedViewModel.A-0.4*A", test,
                () => { test.A = 20; test.NestedViewModel.A = 30; }, "42", (double)42,
                () => { test.A = 30; test.NestedViewModel.A = 10; }, "28", (double)28
            );
        }

        [TestMethod]
        public void NullPropertiesTest()
        {
            var test = new ExampleViewModel();

            test.NestedViewModel = null;

            BoolBindingAssert("NestedViewModel != null", test,
                () => { test.NestedViewModel = null; }, false,
                () => { test.NestedViewModel = new NestedViewModel(); }, true
            );

            StringBindingAssert("(NestedViewModel == null) ? \"a\" : \"b\"", test,
                () => { test.NestedViewModel = null; }, "a",
                () => { test.NestedViewModel = new NestedViewModel(); }, "b"
            );

            BoolBindingAssert("(NestedViewModel != null) && (NestedViewModelOther != null)", test,
                () => { test.NestedViewModel = null; test.NestedViewModelOther = new NestedViewModel(); }, false,
                () => { test.NestedViewModel = new NestedViewModel(); test.NestedViewModelOther = null; }, false
            );

            BoolBindingAssert("(NestedViewModel != null) && (NestedViewModelOther != null)", test,
                () => { test.NestedViewModel = new NestedViewModel(); test.NestedViewModelOther = null; }, false,
                () => { test.NestedViewModelOther = new NestedViewModel(); }, true
            );
        }

        [TestMethod]
        public void NullNullablePropertiesTest()
        {
            var test = new ExampleViewModel();

            test.NullableA = null;

            BoolBindingAssert("NullableA != null", test,
                () => { test.NullableA = null; }, false,
                () => { test.NullableA = 5; }, true
            );

            ObjectBindingAssert("5 + NullableA", test,
                () => { test.NullableA = null; }, null,
                () => { test.NullableA = 5; }, (double?)10
            );

            ObjectBindingAssert("NullableA == 10", test,
                () => { test.NullableA = null; }, null,
                () => { test.NullableA = 10; }, true
            );
        }

        [TestMethod]
        public void PropertyThatChangeItsTypeDuringBinding()
        {
            var test = new ExampleViewModel();

            ObjectBindingAssert("FlickeringViewModel.A", test,
                () => { test.FlickeringViewModel = new ConcreteViewModel1(100); }, (double)100,
                () => { test.FlickeringViewModel = new ConcreteViewModel2(150); }, (double)150
            );
        }

        //--------------------ConvertBack-----------------------------------//

        [TestMethod]
        public void MathBackTest()
        {
            var test = new ExampleViewModel();

            StringAndObjectBindingBackAssert("A+5", test, () => test.A,
                "10.2", "-5", 10.2, -5,
                5.1999999999999993, (double)-10);

            StringAndObjectBindingBackAssert("-B+3.2", test, () => test.B,
                "-3.8", "8.3", -3.8, 8.3,
                7, -5);

            StringAndObjectBindingBackAssert("-4+B", test, () => test.B,
                "3", "5", 3, 5,
                7, 9);

            StringAndObjectBindingBackAssert("((A+5)*3)/4-32", test, () => test.A,
                "10.2", "-5", 10.2, -5,
                51.266666666666673, (double)31);

            // Math

            Console.WriteLine(Math.Sin(31 / 100.0));
            StringAndObjectBindingBackAssert("Math.Sin(B/100.0)", test, () => test.B,
                "0.5", "-0.9", 0.5, -0.9,
                52, -111);

            StringAndObjectBindingBackAssert("Math.Cos(B/100.0)", test, () => test.B,
                "0.5", "-0.9", 0.5, -0.9,
                104, 269);

            StringAndObjectBindingBackAssert("Math.Tan(B/100.0)", test, () => test.B,
                "0.5", "-0.9", 0.5, -0.9,
                46, -73);

            //inversed
            StringAndObjectBindingBackAssert("Math.Asin(B/100.0)", test, () => test.B,
                "0.5", "-0.9", 0.5, -0.9,
                47, -78);

            StringAndObjectBindingBackAssert("Math.Acos(B/100.0)", test, () => test.B,
                "0.5", "-0.9", 0.5, -0.9,
                87, 62);

            StringAndObjectBindingBackAssert("Math.Atan(B/100.0)", test, () => test.B,
                "0.5", "-0.9", 0.5, -0.9,
                54, -126);

            StringAndObjectBindingBackAssert("Math.Pow(B, 4)", test, () => test.B,
                "16", "256", 16, 256,
                2, 4);

            StringAndObjectBindingBackAssert("Math.Pow(4, B)", test, () => test.B,
                "16", "1", 16, 1,
                2, 0);

            StringAndObjectBindingBackAssert("Math.Pow(B, -1)", test, () => test.B,
                "0.1", "0.05", 0.1, 0.05,
                10, 20);

            StringAndObjectBindingBackAssert("Math.Log(B, 5)", test, () => test.B,
                "4", "-3.4", 4, -3.4,
                625, 0);

            StringAndObjectBindingBackAssert("Math.Log(16, B)", test, () => test.B,
                "2", "1", 2, 1,
                4, 16);

            StringAndObjectBindingBackAssert("Math.Sin(Math.Cos(B/100.0))", test, () => test.B,
                "0.8", "-0.2", 0.8, -0.2,
                38, 177);
        }

        [TestMethod]
        public void LogicBackTest()
        {
            var test = new ExampleViewModel();

            BoolBindingBackAssert("!IsChecked", test, () => test.IsChecked,
                true, false,
                false, true);

            StringAndObjectBindingBackAssert("!IsChecked", test, () => test.IsChecked,
                "True", "False", true, false,
                false, true);
        }

        [TestMethod]
        public void VisibilityBackTest()
        {
            var test = new ExampleViewModel();

            VisibilityBindingBackAssert("IsChecked", test, () => test.IsChecked,
                Visibility.Visible, Visibility.Hidden,
                true, false);

            VisibilityBindingBackAssert("IsChecked", test, () => test.IsChecked,
                Visibility.Visible, Visibility.Collapsed,
                true, false);

            VisibilityBindingBackAssert("!IsChecked", test, () => test.IsChecked,
                Visibility.Visible, Visibility.Hidden,
                false, true);

            VisibilityBindingBackAssert("!IsChecked", test, () => test.IsChecked,
                Visibility.Visible, Visibility.Collapsed,
                false, true);
        }

        [TestMethod]
        public void NestedViewModelBackTest()
        {
            var test = new ExampleViewModel();

            StringAndObjectBindingBackAssert("Math.Log(2, NestedViewModel.A)", test, () => test.NestedViewModel.A,
                "0.8", "-0.2", 0.8, -0.2,
                2.3784142300054421, 0.03125);
        }

        [TestMethod]
        public void DataTriggerTest()
        {
            var viewModel = new ExampleViewModel() { A = 1, B = 1 };
            var text = new TextBox
            {
                DataContext = viewModel
            };

            var dataTrigger1 = new DataTrigger
            {
                Value = true
            };

            dataTrigger1.Setters.Add(new Setter { Property = TextBox.BackgroundProperty, Value = Brushes.Red });
            dataTrigger1.Setters.Add(new Setter { Property = TextBox.IsEnabledProperty, Value = true });

            var calcBinding1 = new CalcBinding.Binding("(A+B)>10");

            var bindingExpression1 = calcBinding1.ProvideValue(
                new ServiceProviderMock(dataTrigger1, typeof(DataTrigger).GetProperty("Binding"), null)
                    .WithNullXamlSchemaContextProvider()
            );

            dataTrigger1.Binding = bindingExpression1 as BindingBase;

            var dataTrigger2 = new DataTrigger
            {
                Value = true,
            };

            dataTrigger2.Setters.Add(new Setter { Property = TextBox.BackgroundProperty, Value = Brushes.Green });
            dataTrigger2.Setters.Add(new Setter { Property = TextBox.IsEnabledProperty, Value = false });

            var calcBinding2 = new CalcBinding.Binding("(A+B)<=10");

            var bindingExpression2 = calcBinding2.ProvideValue(
                new ServiceProviderMock(dataTrigger2, typeof(DataTrigger).GetProperty("Binding"), null)
                    .WithNullXamlSchemaContextProvider()
            );

            dataTrigger2.Binding = bindingExpression2 as BindingBase;

            var style = new Style();
            style.Triggers.Add(dataTrigger1);
            style.Triggers.Add(dataTrigger2);

            text.Style = style;

            // act, assert
            viewModel.A = 5;
            viewModel.B = 7;

            Assert.AreEqual(Brushes.Red, text.Background);
            Assert.AreEqual(true, text.IsEnabled);

            viewModel.B = 2;

            Assert.AreEqual(Brushes.Green, text.Background);
            Assert.AreEqual(false, text.IsEnabled);
        }

        [TestMethod]
        public void ParsePseudonimsOfOperatorsAsPartOfPropertiesNamesTest()
        {
            //-------------------------------------------------------------------//
            var test = new ExampleViewModel();

            //-------------------------------------------------------------------//
            ObjectBindingAssert("PorksCount*2", test,
                () => { test.PorksCount = 2; }, 4,
                () => { test.PorksCount = 4; }, 8
            );

            ObjectBindingAssert("Pandus*2", test,
                () => { test.Pandus = 2; }, 4,
                () => { test.Pandus = 4; }, 8
            );

            ObjectBindingAssert("Fairlessly*2", test,
                () => { test.Fairlessly = 2; }, 4,
                () => { test.Fairlessly = 4; }, 8
            );

            ObjectBindingAssert("Fairless==5", test,
                () => { test.Fairless = 5; }, true,
                () => { test.Fairless = 4; }, false
            );
        }

        [TestMethod]
        public void ParsePseudonimsWithBracketsWithoutSpacesTest()
        {
            //-------------------------------------------------------------------//
            var test = new ExampleViewModel();

            //-------------------------------------------------------------------//
            ObjectBindingAssert("(A>B)or(B > C)", test,
                () => { test.A = 5; test.B = 4; test.C = 3; }, true,
                () => { test.A = 5; test.B = 7; test.C = 9; }, false
            );

            ObjectBindingAssert("(A>B)and(B > C)", test,
                () => { test.A = 5; test.B = 4; test.C = 3; }, true,
                () => { test.A = 2; }, false
            );
            ObjectBindingAssert("(A+B)less(B + C)", test,
                () => { test.A = 5; test.B = 4; test.C = 10; }, true,
                () => { test.C = 2; }, false
            );
            ObjectBindingAssert("(A+B)less=(B + C)", test,
                () => { test.A = 5; test.B = 4; test.C = 5; }, true,
                () => { test.C = 3; }, false
            );
        }

        [TestMethod]
        public void ParsePseudonimsWithBracketsWithSpacesTest()
        {
            //-------------------------------------------------------------------//
            var test = new ExampleViewModel();

            //-------------------------------------------------------------------//
            ObjectBindingAssert("(A>B) or(B > C)", test,
                () => { test.A = 5; test.B = 4; test.C = 3; }, true,
                () => { test.A = 5; test.B = 7; test.C = 9; }, false
            );
            ObjectBindingAssert("(A>B)or (B > C)", test,
                () => { test.A = 5; test.B = 4; test.C = 3; }, true,
                () => { test.A = 5; test.B = 7; test.C = 9; }, false
            );
            ObjectBindingAssert("(A>B) or (B > C)", test,
                () => { test.A = 5; test.B = 4; test.C = 3; }, true,
                () => { test.A = 5; test.B = 7; test.C = 9; }, false
            );

            ObjectBindingAssert("(A>B) and(B > C)", test,
                () => { test.A = 5; test.B = 4; test.C = 3; }, true,
                () => { test.A = 2; }, false
            );
            ObjectBindingAssert("(A>B)and (B > C)", test,
                () => { test.A = 5; test.B = 4; test.C = 3; }, true,
                () => { test.A = 2; }, false
            );
            ObjectBindingAssert("(A>B) and (B > C)", test,
                () => { test.A = 5; test.B = 4; test.C = 3; }, true,
                () => { test.A = 2; }, false
            );

            ObjectBindingAssert("(A+B) less(B + C)", test,
                () => { test.A = 5; test.B = 4; test.C = 10; }, true,
                () => { test.C = 2; }, false
            );
            ObjectBindingAssert("(A+B)less (B + C)", test,
                () => { test.A = 5; test.B = 4; test.C = 10; }, true,
                () => { test.C = 2; }, false
            );
            ObjectBindingAssert("(A+B) less (B + C)", test,
                () => { test.A = 5; test.B = 4; test.C = 10; }, true,
                () => { test.C = 2; }, false
            );

            ObjectBindingAssert("(A+B) less=(B + C)", test,
                () => { test.A = 5; test.B = 4; test.C = 5; }, true,
                () => { test.C = 3; }, false
            );
            ObjectBindingAssert("(A+B)less= (B + C)", test,
                () => { test.A = 5; test.B = 4; test.C = 5; }, true,
                () => { test.C = 3; }, false
            );
            ObjectBindingAssert("(A+B) less= (B + C)", test,
                () => { test.A = 5; test.B = 4; test.C = 5; }, true,
                () => { test.C = 3; }, false
            );
        }

        [TestMethod]
        public void ParseLessEqualOperatorTest()
        {
            //-------------------------------------------------------------------//
            var test = new ExampleViewModel();

            //-------------------------------------------------------------------//
            ObjectBindingAssert("A <= B", test,
                () => { test.A = 5; test.B = 4; }, false,
                () => { test.A = 5; test.B = 7; }, true
            );
        }

        [TestMethod]
        public void ParseGreaterEqualOperatorTest()
        {
            //-------------------------------------------------------------------//
            var test = new ExampleViewModel();

            //-------------------------------------------------------------------//
            ObjectBindingAssert("A >= B", test,
                () => { test.A = 3; test.B = 4; }, false,
                () => { test.A = 7; test.B = 5; }, true
            );
        }

        [TestMethod]
        public void ParseEqualOperatorTest()
        {
            //-------------------------------------------------------------------//
            var test = new ExampleViewModel();

            //-------------------------------------------------------------------//
            ObjectBindingAssert("A == B", test,
                () => { test.A = 4; test.B = 4; }, true,
                () => { test.A = 7; test.B = 5; }, false
            );
        }

        [TestMethod]
        public void ParseNotEqualOperatorTest()
        {
            //-------------------------------------------------------------------//
            var test = new ExampleViewModel();

            //-------------------------------------------------------------------//
            ObjectBindingAssert("A != B", test,
                () => { test.A = 3; test.B = 4; }, true,
                () => { test.A = 5; test.B = 5; }, false
            );
        }

        [TestMethod]
        public void MPropertyWithMathBindingTest()
        {
            var test = new ExampleViewModel();

            StringAndObjectBindingAssert("M + Math.Abs(M)", test,
                () => test.M = 10, "20", (double)20,
                () => test.M = -10, "0", (double)0
            );
        }

        [TestMethod]
        public void BindingToPropertyContainingDigits()
        {
            var test = new ExampleViewModel();

            StringAndObjectBindingAssert("N1", test,
                () => test.N1 = 10, "10", (double)10,
                () => test.N1 = 20.34, "20.34", 20.34
            );
        }

        /// <summary>
        /// Defect https://github.com/Alex141/CalcBinding/issues/32
        /// </summary>
        [TestMethod]
        public void BindingToStringWithParenteses()
        {
            var test = new ExampleViewModel();

            StringBindingAssert("Name == null ? '(new)' : 'Name = ' + Name", test,
                () => test.Name = null, "(new)",
                () => test.Name = "ViewModel", "Name = ViewModel"
            );
        }
    }
}
