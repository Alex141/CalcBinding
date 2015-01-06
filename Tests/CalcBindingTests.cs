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

namespace Tests
{
    public class ServiceProviderMock : IServiceProvider
    {
        ProvideValueTargetMock provideValueTargetMock;

        public ServiceProviderMock(object TargetObject, object TargetProperty)
        {
            provideValueTargetMock = new ProvideValueTargetMock
            {
                TargetObject = TargetObject,
                TargetProperty = TargetProperty
            };
        }

        public object GetService(Type serviceType)
        {
            return provideValueTargetMock;
        }
    }

    public class ProvideValueTargetMock : IProvideValueTarget
    {
        public object TargetObject { get; set; }
        public object TargetProperty { get; set; }

        public ProvideValueTargetMock()
        {
        }
    }

    /// <summary>
    /// System tests
    /// </summary>
    [TestClass]
    public class CalcBindingTests
    {
        // что нужно передать для тестирования:
        // биндинг к чему мы тестируем? (String, object, Visibility, bool)
        // => создаётся: TextBox.Text, Label.Content, CheckBox.Visibility, CheckBox.IsChecked
        // PropertyInfo изменяемого свойства
        // Начальное значение в Source, что должны увидеть в xaml? 
        // На что потом меняем. Что должны увидеть в xaml? 
        // Меняем ли xaml? 
        // На что меняем? Что должны ожидать в source?
        [TestMethod]
        public void MathPropertyBindingTest()
        {
            var test = new ExampleViewModel();

            StringAndObjectBindingAssert("A", test,
                () => test.A = 10, "10", (double)10,
                () => test.A = 20.34, "20.34", 20.34
            );

            // bug detected. Bug resolved
            StringAndObjectBindingAssert("A+B+C", test,
                () => { test.A = 10; test.B = 20; test.C = -2; }, "28", (double)28,
                () => { test.A = 20.34; test.B = 15; test.C = 12; }, "47.34", 47.34
            );

            StringAndObjectBindingAssert("A-B-C", test,
                () => { test.A = 10; test.B = 20; test.C = 5; }, "-15", (double)-15,
                () => { test.A = 5; test.B = 3; test.C = -7; }, "9", (double)9
            );

            // real))
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

            var calcBinding = new CalcBinding.Binding("A/B")
            {
                StringFormat = "{0:n2}"
            };

            StringBindingAssert(calcBinding, test,
                () => { test.A = 10; test.B = 3; }, "3.33",
                () => { test.A = 20; test.B = -30; }, "-0.67"
            );
            // todo: test for string format. Need tuned CalcBinding
        }

        [TestMethod]
        public void LogicPropertyBindingTest()
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
        public void VisibilityPropertyBindingTest()
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

        //todo: сделать тесты на то, что вызывается компиляция 1 раз. Интерфейс для Interpreter.
        // как задать? В конструкторе. В его перегрузке задавать по умолчанию
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
    }

}
