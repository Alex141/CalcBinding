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
    //[TestClass]
    //public class CalcBindingTests
    //{
    //}

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
        public void OnePropertyBindingTest()
        {
            var test = new ExampleViewModel();

            StringBindingAssert("A", test, () => test.A = 10, "10", () => test.A = 20.34, "20.34");
            StringBindingAssert("A+B+C", test, 
            () => {
                test.A = 10; 
                test.B = 20;
            },"10", 
            () => {
                test.A = 20.34;
                test.B = 15;
            }, "35.34");
            // просто набивание кода. Здесь надо перекопировать всю страничку с примерами.
            // Этого будет достаточно для тестов

            //BoolBindingAssert("", test, () => test.A = 10, "10", () => test.A = 20, "20");
            //StringBindingAssert("A", test, () => test.A = 10, "10", () => test.A = 20, "20");
            //StringBindingAssert("A", test, () => test.A = 10, "10", () => test.A = 20, "20");
        }

        //todo: сделать тесты на то, что вызывается компиляция 1 раз. Интерфейс для Interpreter.
        // как задать? В конструкторе. В его перегрузке задавать по умолчанию

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

        public void VisibilityBindingAssert(string path, INotifyPropertyChanged source,
                    Action sourcePropertySetter1, Visibility targetValue1,
                    Action sourcePropertySetter2, Visibility targetValue2)
        {
            var label = new Label();
            BindingAssert(path, source, label, Label.VisibilityProperty, () => label.Visibility, sourcePropertySetter1, targetValue1, sourcePropertySetter2, targetValue2);
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
            //var test = new ExampleViewModel();

            targetObject.DataContext = source;

            //var sourcePropertyName = GetMemberName(property);
            var calcBinding = new CalcBinding.Binding(path);

            var bindingExpression = calcBinding.ProvideValue(new ServiceProviderMock(targetObject, targetProperty));

            targetObject.SetValue(targetProperty, bindingExpression);

            //act
            sourcePropertySetter1();

            //assert
            Assert.AreEqual(targetValue1, targetPropertyGetter());

            //act
            sourcePropertySetter2();

            //assert
            Assert.AreEqual(targetValue2, targetPropertyGetter());
        }

        public string GetMemberName<T>(Expression<Func<T>> func)
        {
            return (func.Body as MemberExpression).Member.Name;
        }
    }

}
