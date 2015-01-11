using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using WpfExample;
using Tests.Mocks;

namespace Tests
{
    /// <summary>
    /// System tests
    /// </summary>
    [TestClass]
    public class CalcBindingSystemTests
    {
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
                () => { test.A = 5.4; test.B = 3; test.C = -8; }, "59.4", (double)59.400000000000006
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
                () => { test.A = 5;             }, "20", 20
            );

            BoolBindingAssert("!IsChecked", test,
                () => test.IsChecked = true, false,
                () => test.IsChecked = false, true
            );

            BoolBindingAssert("!IsChecked and IsFull", test,
                () => { test.IsChecked = false; test.IsFull = false; }, false,
                () => { test.IsChecked = false; test.IsFull = true; }, true
            );

            BoolBindingAssert("!IsChecked && IsFull", test,
                () => { test.IsChecked = false; test.IsFull = false; }, false,
                () => { test.IsChecked = false; test.IsFull = true; }, true
            );

            BoolBindingAssert("!IsChecked or (A > B)", test,
                () => { test.IsChecked = false; test.A = 10; test.B = 20; }, true,
                () => { test.IsChecked = true; test.A = 5; }, false
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
                () => { test.A = 1; test.B = 10; }, false,
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
                FalseToVisibility = CalcBinding.FalseToVisibility.Collapsed
            };

            VisibilityBindingAssert(calcBinding, test,
                () => { test.HasPrivileges = true; }, Visibility.Collapsed,
                () => { test.HasPrivileges = false; }, Visibility.Visible
            );

            //-------------------------------------------------------------------//
            calcBinding = new CalcBinding.Binding("!HasPrivileges")
            {
                FalseToVisibility = CalcBinding.FalseToVisibility.Hidden
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

            Console.WriteLine(Math.Sin(31/100.0));
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

        //todo: подумать насчёт обработки ошибок. Всё таки, что делать с ними?

        //наверное надо рассуждать так: Если ошибка в Path, то сразу сигналим,
        // т.к. это хер исправишь. А если в данных - то ничего, забить.
        // с другой стороны стандартный биндинг не ругается даже когда в Path ошибка,
        // он в этом плане вообще туповат. Я могу взять пример с него, писать
        // такие же ошибки (от конвертера): Binding (CalcBinding): ssdfsf sdfsdf sdf.
        // тогда мой биндинг будет не отличить от старого. Но добавлю своего немного)

        #region Convert

        public void StringAndObjectBindingAssert(string path, INotifyPropertyChanged source,
    Action sourcePropertySetter1, string targetValue1, object objTargetValue1,
    Action sourcePropertySetter2, string targetValue2, object objTargetValue2)
        {
            StringBindingAssert(path, source, sourcePropertySetter1, targetValue1, sourcePropertySetter2, targetValue2);
            ObjectBindingAssert(path, source, sourcePropertySetter1, objTargetValue1, sourcePropertySetter2, objTargetValue2);
        }

        public void StringBindingAssert(CalcBinding.Binding calcBinding, INotifyPropertyChanged source,
            Action sourcePropertySetter1, string targetValue1,
            Action sourcePropertySetter2, string targetValue2)
        {
            var textBox = new TextBox();
            BindingAssert(calcBinding, source, textBox, TextBox.TextProperty, () => textBox.Text, sourcePropertySetter1, targetValue1, sourcePropertySetter2, targetValue2);
        }

        public void StringBindingAssert(string path, INotifyPropertyChanged source,
            Action sourcePropertySetter1, string targetValue1,
            Action sourcePropertySetter2, string targetValue2)
        {
            var textBox = new TextBox();
            BindingAssert(path, source, textBox, TextBox.TextProperty, () => textBox.Text, sourcePropertySetter1, targetValue1, sourcePropertySetter2, targetValue2);
        }

        public void ObjectBindingAssert(string path, INotifyPropertyChanged source,
                    Action sourcePropertySetter1, object targetValue1,
                    Action sourcePropertySetter2, object targetValue2)
        {
            var label = new Label();
            BindingAssert(path, source, label, Label.ContentProperty, () => label.Content, sourcePropertySetter1, targetValue1, sourcePropertySetter2, targetValue2);
        }

        public void VisibilityBindingAssert(CalcBinding.Binding calcBinding, INotifyPropertyChanged source,
                    Action sourcePropertySetter1, Visibility targetValue1,
                    Action sourcePropertySetter2, Visibility targetValue2)
        {
            var label = new Label();
            BindingAssert(calcBinding, source, label, Label.VisibilityProperty, () => label.Visibility, sourcePropertySetter1, targetValue1, sourcePropertySetter2, targetValue2);
        }

        public void VisibilityBindingAssert(string path, INotifyPropertyChanged source,
                    Action sourcePropertySetter1, Visibility targetValue1,
                    Action sourcePropertySetter2, Visibility targetValue2)
        {
            var label = new Label();
            BindingAssert(path, source, label, Label.VisibilityProperty, () => label.Visibility, sourcePropertySetter1, targetValue1, sourcePropertySetter2, targetValue2);
        }

        public void BoolBindingAssert(CalcBinding.Binding calcBinding, INotifyPropertyChanged source,
                    Action sourcePropertySetter1, bool targetValue1,
                    Action sourcePropertySetter2, bool targetValue2)
        {
            var checkbox = new CheckBox();
            BindingAssert(calcBinding, source, checkbox, CheckBox.IsCheckedProperty, () => checkbox.IsChecked, sourcePropertySetter1, targetValue1, sourcePropertySetter2, targetValue2);
        }

        public void BoolBindingAssert(string path, INotifyPropertyChanged source,
                    Action sourcePropertySetter1, bool targetValue1,
                    Action sourcePropertySetter2, bool targetValue2)
        {
            var checkbox = new CheckBox();
            BindingAssert(path, source, checkbox, CheckBox.IsCheckedProperty, () => checkbox.IsChecked, sourcePropertySetter1, targetValue1, sourcePropertySetter2, targetValue2);
        }

        public void BindingAssert<TTargetProperty>(
            string path, INotifyPropertyChanged source,
            FrameworkElement targetObject, DependencyProperty targetProperty,
            Func<TTargetProperty> targetPropertyGetter,
            Action sourcePropertySetter1, TTargetProperty targetValue1,
            Action sourcePropertySetter2, TTargetProperty targetValue2
            )
        {
            var calcBinding = new CalcBinding.Binding(path);

            BindingAssert(calcBinding, source, targetObject, targetProperty, targetPropertyGetter,
                sourcePropertySetter1, targetValue1,
                sourcePropertySetter2, targetValue2);
        }

        public void BindingAssert<TTargetProperty>(
            CalcBinding.Binding calcBinding, INotifyPropertyChanged source,
            FrameworkElement targetObject, DependencyProperty targetProperty,
            Func<TTargetProperty> targetPropertyGetter,
            Action sourcePropertySetter1, TTargetProperty targetValue1,
            Action sourcePropertySetter2, TTargetProperty targetValue2
            )
        {
            //var test = new ExampleViewModel();

            targetObject.DataContext = source;

            var bindingExpression = calcBinding.ProvideValue(new ServiceProviderMock(targetObject, targetProperty));

            targetObject.SetValue(targetProperty, bindingExpression);

            //act
            sourcePropertySetter1();

            //assert
            var realValue1 = targetPropertyGetter();
            Assert.AreEqual(targetValue1, realValue1);

            //act
            sourcePropertySetter2();

            //assert
            var realValue2 = targetPropertyGetter();
            Assert.AreEqual(targetValue2, realValue2);
        }

        public string GetMemberName<T>(Expression<Func<T>> func)
        {
            return (func.Body as MemberExpression).Member.Name;
        } 

        #endregion

        #region ConvertBack

        public void StringAndObjectBindingBackAssert(string path, INotifyPropertyChanged source, Func<object> sourcePropertyGetter,
            String stringTargetValue1, String stringTargetValue2,
            object objTargetValue1, object objTargetValue2,
            object sourcePropertyValue1, object sourcePropertyValue2)
        {
            StringBindingBackAssert(path, source, sourcePropertyGetter, stringTargetValue1, stringTargetValue2, sourcePropertyValue1, sourcePropertyValue2);
            ObjectBindingBackAssert(path, source, sourcePropertyGetter, objTargetValue1, objTargetValue2, sourcePropertyValue1, sourcePropertyValue2);
        }

        public void VisibilityBindingBackAssert(string path, INotifyPropertyChanged source, Func<object> sourcePropertyGetter,
            Visibility targetValue1, Visibility targetValue2,
            object sourcePropertyValue1, object sourcePropertyValue2)
        {
            var obj = new Label();
            BindingBackAssert(path, source, sourcePropertyGetter,
                obj, Label.VisibilityProperty,
                (targetValue) => obj.Visibility = targetValue, 
                targetValue1, targetValue2,
                sourcePropertyValue1, sourcePropertyValue2);
        }

        public void BoolBindingBackAssert(string path, INotifyPropertyChanged source, Func<object> sourcePropertyGetter,
            bool targetValue1, bool targetValue2,
            object sourcePropertyValue1, object sourcePropertyValue2)
        {
            var obj = new CheckBox();
            BindingBackAssert(path, source, sourcePropertyGetter,
                obj, CheckBox.IsCheckedProperty,
                (targetValue) => obj.IsChecked = targetValue, 
                targetValue1, targetValue2,
                sourcePropertyValue1, sourcePropertyValue2);
        }

        public void ObjectBindingBackAssert(string path, INotifyPropertyChanged source, Func<object> sourcePropertyGetter,
            object targetValue1, object targetValue2,
            object sourcePropertyValue1, object sourcePropertyValue2)
        {
            var obj = new Button();
            BindingBackAssert(path, source, sourcePropertyGetter, 
                obj, Button.ContentProperty, 
                (targetValue) => obj.Content = targetValue, 
                targetValue1, targetValue2, 
                sourcePropertyValue1, sourcePropertyValue2);
        }

        public void StringBindingBackAssert(string path, INotifyPropertyChanged source, Func<object> sourcePropertyGetter,
            String targetValue1, String targetValue2,
            object sourcePropertyValue1, object sourcePropertyValue2)
        {
            var obj = new TextBox();
            BindingBackAssert(path, source, sourcePropertyGetter, 
                obj, TextBox.TextProperty,
                (targetValue) => obj.Text = targetValue,
                targetValue1, targetValue2,
                sourcePropertyValue1, sourcePropertyValue2);
        }

        public void BindingBackAssert<TTargetProperty>(
    string path, INotifyPropertyChanged source, Func<object> sourcePropertyGetter,
    FrameworkElement targetObject, DependencyProperty targetProperty,
    Action<TTargetProperty> targetPropertySetter,
    TTargetProperty targetPropertyValue1, TTargetProperty targetPropertyValue2,
    object sourcePropertyValue1, object sourcePropertyValue2)
        {
            var calcBinding = new CalcBinding.Binding(path)
            {
                UpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged
            };

            BindingBackAssert(calcBinding, source, sourcePropertyGetter, 
                targetObject, targetProperty, targetPropertySetter, 
                targetPropertyValue1, targetPropertyValue2,
                sourcePropertyValue1, sourcePropertyValue2);
        }

        public void BindingBackAssert<TTargetProperty>(
    CalcBinding.Binding calcBinding, 
    INotifyPropertyChanged source, Func<object> sourcePropertyGetter,
    FrameworkElement targetObject, DependencyProperty targetProperty,
    Action<TTargetProperty> targetPropertySetter,
    TTargetProperty targetPropertyValue1, TTargetProperty targetPropertyValue2,
    object sourcePropertyValue1, object sourcePropertyValue2)
        {
            targetObject.DataContext = source;

            var bindingExpression = calcBinding.ProvideValue(new ServiceProviderMock(targetObject, targetProperty));

            targetObject.SetValue(targetProperty, bindingExpression);

            //act
            targetPropertySetter(targetPropertyValue1);

            //assert
            var realSourcePropertyValue1 = sourcePropertyGetter();
            Assert.AreEqual(sourcePropertyValue1, realSourcePropertyValue1);

            //act
            targetPropertySetter(targetPropertyValue2);

            //assert
            var realSourcePropertyValue2 = sourcePropertyGetter();
            Assert.AreEqual(sourcePropertyValue2, realSourcePropertyValue2);
        } 

        #endregion
    }

}
