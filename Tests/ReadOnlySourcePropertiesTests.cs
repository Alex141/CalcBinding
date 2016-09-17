using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WpfExample;

namespace Tests
{
    [TestClass]
    public class ReadOnlySourcePropertiesTests:BaseSystemTests
    {
        #region Binding Label.Content

        [TestMethod]
        public void BindingLabelContentToReadonlyPropertyWithoutSettingModeSuccessTest()
        {
            var metadata = Label.ContentProperty.GetMetadata(typeof(Label));
            Assert.IsInstanceOfType(metadata, typeof(FrameworkPropertyMetadata), "Metadata of Label should be FrameworkMetadata");
            Assert.AreEqual(false, (metadata as FrameworkPropertyMetadata).BindsTwoWayByDefault);

            var calcBinding = new CalcBinding.Binding("ReadonlyA")
            {
                UpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged
            };

            var source = new ExampleViewModel();
            var element = new Label();

            BindingBackAssert(calcBinding, source, () => source.A,
                element, Label.ContentProperty, (string s) => element.Content = s,
                "10", "100",
                (double)10, (double)10);
        }

        [TestMethod]
        public void BindingLabelContentToReadonlyPropertyWithDefaultModeFailsTest()
        {
            var metadata = Label.ContentProperty.GetMetadata(typeof(Label));
            Assert.IsInstanceOfType(metadata, typeof(FrameworkPropertyMetadata), "Metadata of Label should be FrameworkMetadata");
            Assert.AreEqual(false, (metadata as FrameworkPropertyMetadata).BindsTwoWayByDefault);

            var calcBinding = new CalcBinding.Binding("ReadonlyA")
            {
                UpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged,
                Mode = BindingMode.Default
            };

            var source = new ExampleViewModel();
            var element = new Label();

            BindingBackAssert(calcBinding, source, () => source.A,
                element, Label.ContentProperty, (string s) => element.Content = s,
                "10", "100",
                (double)10, (double)10);
        }

        [TestMethod]
#if NET45
        [ExpectedExceptionEx(exceptionType: typeof(InvalidOperationException), hResult: -2146233079)]
#else
        [ExpectedException(exceptionType: typeof(InvalidOperationException))]
#endif
        public void BindingLabelContentToReadonlyPropertyWithTwoWayModeFailsTest()
        {
            var metadata = Label.ContentProperty.GetMetadata(typeof(Label));
            Assert.IsInstanceOfType(metadata, typeof(FrameworkPropertyMetadata), "Metadata of Label should be FrameworkMetadata");
            Assert.AreEqual(false, (metadata as FrameworkPropertyMetadata).BindsTwoWayByDefault);

            var calcBinding = new CalcBinding.Binding("ReadonlyA")
            {
                UpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged,
                Mode = BindingMode.TwoWay
            };

            var source = new ExampleViewModel();
            var element = new Label();

            BindingBackAssert(calcBinding, source, () => source.A,
                element, Label.ContentProperty, (string s) => element.Content = s,
                "10", "100",
                (double)10, (double)100);
        }

        [TestMethod]
        public void BindingLabelContentToReadonlyPropertyWithOneWayModeSuccessTest()
        {
            var metadata = Label.ContentProperty.GetMetadata(typeof(Label));
            Assert.IsInstanceOfType(metadata, typeof(FrameworkPropertyMetadata), "Metadata of Label should be FrameworkMetadata");
            Assert.AreEqual(false, (metadata as FrameworkPropertyMetadata).BindsTwoWayByDefault);

            var calcBinding = new CalcBinding.Binding("ReadonlyA")
            {
                UpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged,
                Mode = BindingMode.OneWay
            };

            var source = new ExampleViewModel();
            var element = new Label();

            BindingBackAssert(calcBinding, source, () => source.A,
                element, Label.ContentProperty, (string s) => element.Content = s,
                "10", "100",
                (double)10, (double)10);
        }

        #endregion


        #region Binding TextBox.Text

        [TestMethod]
#if NET45
        [ExpectedExceptionEx(exceptionType: typeof(InvalidOperationException), hResult: -2146233079)]
#else
        [ExpectedException(exceptionType: typeof(InvalidOperationException))]
#endif
        public void BindingTextBoxToReadonlyPropertyWithoutSettingModeFailsTest()
        {
            var metadata = TextBox.TextProperty.GetMetadata(typeof(TextBox));
            Assert.IsInstanceOfType(metadata, typeof(FrameworkPropertyMetadata), "Metadata of TextBox should be FrameworkMetadata");
            Assert.AreEqual(true, (metadata as FrameworkPropertyMetadata).BindsTwoWayByDefault);

            var calcBinding = new CalcBinding.Binding("ReadonlyA")
            {
                UpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged
            };

            var source = new ExampleViewModel();
            var element = new TextBox();

            BindingBackAssert(calcBinding, source, () => source.A,
                element, TextBox.TextProperty, (string s) => element.Text = s,
                "10", "100",
                (double)10, (double)100);
        }

#if NET45
        [ExpectedExceptionEx(exceptionType: typeof(InvalidOperationException), hResult: -2146233079)]
#else
        [ExpectedException(exceptionType: typeof(InvalidOperationException))]
#endif
        [TestMethod]
        public void BindingTextBoxToReadonlyPropertyWithDefaultModeFailsTest()
        {
            var metadata = TextBox.TextProperty.GetMetadata(typeof(TextBox));
            Assert.IsInstanceOfType(metadata, typeof(FrameworkPropertyMetadata), "Metadata of TextBox should be FrameworkMetadata");
            Assert.AreEqual(true, (metadata as FrameworkPropertyMetadata).BindsTwoWayByDefault);

            var calcBinding = new CalcBinding.Binding("ReadonlyA")
            {
                UpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged,
                Mode = BindingMode.Default
            };

            var source = new ExampleViewModel();
            var element = new TextBox();

            BindingBackAssert(calcBinding, source, () => source.A,
                element, TextBox.TextProperty, (string s) => element.Text = s,
                "10", "100",
                (double)10, (double)10);
        }

        [TestMethod]
        public void BindingTextBoxToReadonlyPropertyWithOneWayModeSuccessTest()
        {
            var metadata = TextBox.TextProperty.GetMetadata(typeof(TextBox));
            Assert.IsInstanceOfType(metadata, typeof(FrameworkPropertyMetadata), "Metadata of TextBox should be FrameworkMetadata");
            Assert.AreEqual(true, (metadata as FrameworkPropertyMetadata).BindsTwoWayByDefault);

            var calcBinding = new CalcBinding.Binding("ReadonlyA")
            {
                UpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged,
                Mode = BindingMode.OneWay
            };

            var source = new ExampleViewModel();
            var element = new TextBox();

            BindingBackAssert(calcBinding, source, () => source.A,
                element, TextBox.TextProperty, (string s) => element.Text = s,
                "10", "100",
                (double)10, (double)10);
        }

        [TestMethod]
#if NET45
        [ExpectedExceptionEx(exceptionType: typeof(InvalidOperationException), hResult: -2146233079)]
#else
        [ExpectedException(exceptionType: typeof(InvalidOperationException))]
#endif
        public void BindingTextBoxToReadonlyPropertyWithOneWayToSourceFailsTest()
        {
            var metadata = TextBox.TextProperty.GetMetadata(typeof(TextBox));
            Assert.IsInstanceOfType(metadata, typeof(FrameworkPropertyMetadata), "Metadata of TextBox should be FrameworkMetadata");
            Assert.AreEqual(true, (metadata as FrameworkPropertyMetadata).BindsTwoWayByDefault);

            var calcBinding = new CalcBinding.Binding("ReadonlyA")
            {
                UpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged,
                Mode = BindingMode.OneWayToSource
            };

            var source = new ExampleViewModel();
            var element = new TextBox();

            BindingBackAssert(calcBinding, source, () => source.A,
                element, TextBox.TextProperty, (string s) => element.Text = s,
                "10", "100",
                (double)10, (double)100);
        }

        #endregion
    }
}
