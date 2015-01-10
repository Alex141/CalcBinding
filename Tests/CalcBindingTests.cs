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
    public class CalcBindingSystemTests
    {
        [TestMethod]
        public void MathPropertyTest()
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

            var calcBinding = new CalcBinding.Binding("A/B")
            {
                StringFormat = "{0:n2}"
            };

            StringBindingAssert(calcBinding, test,
                () => { test.A = 10; test.B = 3; }, "3.33",
                () => { test.A = 20; test.B = -30; }, "-0.67"
            );
        }

        [TestMethod] 
        public void StringPropertyTest()
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
        public void LogicPropertyTest()
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
        public void VisibilityPropertyTest()
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
        //todo: сделать тесты на то, что вызывается компиляция 1 раз. Интерфейс для Interpreter.
        // как задать? В конструкторе. В его перегрузке задавать по умолчанию
        
        //todo: тесты на inverse ( естественно с одной переменной )
        //todo: тесты на inverse с множеством операций привидения типов.
        //todo: вообще еще раз разобраться что там и как работает, именно на последнем и первом
        // этапах, мне кажется что в текущем виде у нас не заработает
        //todo: подумать насчёт обработки ошибок. Всё таки, что делать с ними?

        //наверное надо рассуждать так: Если ошибка в Path, то сразу сигналим,
        // т.к. это хер исправишь. А если в данных - то ничего, забить.
        // с другой стороны стандартный биндинг не ругается даже когда в Path ошибка,
        // он в этом плане вообще туповат. Я могу взять пример с него, писать
        // такие же ошибки (от конвертера): Binding (CalcBinding): ssdfsf sdfsdf sdf.
        // тогда мой биндинг будет не отличить от старого. Но добавлю своего немного)

        // todo: для тестов на инверсию ОБЯЗАТЕЛЬНЫ тесты на инверсии каждой операции, в том
        // числе и Negate (унарный минус) и Not (логическое отрицание)
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
