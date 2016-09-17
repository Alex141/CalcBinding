using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Tests.Mocks;

namespace Tests
{
    public class BaseSystemTests
    {
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
            LabelBindingAssert(path, source, sourcePropertySetter1, targetValue1, sourcePropertySetter2, targetValue2);
        }

        public void LabelBindingAssert(string path, INotifyPropertyChanged source,
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
                sourcePropertyValue1, sourcePropertyValue2, BindingMode.TwoWay);
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
                sourcePropertyValue1, sourcePropertyValue2, BindingMode.Default);
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
                sourcePropertyValue1, sourcePropertyValue2, BindingMode.TwoWay);
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
                sourcePropertyValue1, sourcePropertyValue2, BindingMode.Default);
        }

        public void BindingBackAssert<TTargetProperty>(
    string path, INotifyPropertyChanged source, Func<object> sourcePropertyGetter,
    FrameworkElement targetObject, DependencyProperty targetProperty,
    Action<TTargetProperty> targetPropertySetter,
    TTargetProperty targetPropertyValue1, TTargetProperty targetPropertyValue2,
    object sourcePropertyValue1, object sourcePropertyValue2, BindingMode mode)
        {
            var calcBinding = new CalcBinding.Binding(path)
            {
                UpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged,
                Mode = mode
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
