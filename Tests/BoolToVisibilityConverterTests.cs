using CalcBinding;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Tests
{
    [TestClass]
    public class BoolToVisibilityConverterTests
    {
        [TestMethod]
        public void ConvertBoolToVisibilityTest()
        {
            var converter = new CalcConverter();

            Assert.AreEqual(Visibility.Visible,
                new BoolToVisibilityConverter()
                .Convert(true, typeof(Visibility), null, CultureInfo.CurrentCulture));

            Assert.AreEqual(Visibility.Collapsed,
                new BoolToVisibilityConverter()
                .Convert(false, typeof(Visibility), null, CultureInfo.CurrentCulture));

            Assert.AreEqual(Visibility.Hidden,
                new BoolToVisibilityConverter(FalseToVisibility.Hidden)
                .Convert(false, typeof(Visibility), null, CultureInfo.CurrentCulture));
        }

        [TestMethod]
        public void ConvertVisibilityToBoolTest()
        {
            Assert.AreEqual(true,
                new BoolToVisibilityConverter()
                .ConvertBack(Visibility.Visible, typeof(bool), null, CultureInfo.CurrentCulture));

            Assert.AreEqual(false,
                new BoolToVisibilityConverter()
                .ConvertBack(Visibility.Collapsed, typeof(bool), null, CultureInfo.CurrentCulture));

            Assert.AreEqual(false,
                new BoolToVisibilityConverter()
                .ConvertBack(Visibility.Hidden, typeof(bool), null, CultureInfo.CurrentCulture));
        }

        [TestMethod]
        public void ConvertsCastableToBoolWhenValueHasImplicitConversion()
        {
            Assert.AreEqual(Visibility.Visible,
                new BoolToVisibilityConverter()
                .Convert(new ImplicitCastableToBoolean(true), typeof(Visibility), null, CultureInfo.CurrentCulture));

            Assert.AreEqual(Visibility.Collapsed,
                new BoolToVisibilityConverter()
                .Convert(new ImplicitCastableToBoolean(false), typeof(Visibility), null, CultureInfo.CurrentCulture));

            Assert.AreEqual(Visibility.Hidden,
                new BoolToVisibilityConverter(FalseToVisibility.Hidden)
                .Convert(new ImplicitCastableToBoolean(false), typeof(Visibility), null, CultureInfo.CurrentCulture));
        }

        [TestMethod]
        public void ConvertsCastableToBoolWhenValueHasExplicitConversion()
        {
            Assert.AreEqual(Visibility.Visible,
                new BoolToVisibilityConverter()
                .Convert(new ExplicitCastableToBoolean(true), typeof(Visibility), null, CultureInfo.CurrentCulture));

            Assert.AreEqual(Visibility.Collapsed,
                new BoolToVisibilityConverter()
                .Convert(new ExplicitCastableToBoolean(false), typeof(Visibility), null, CultureInfo.CurrentCulture));

            Assert.AreEqual(Visibility.Hidden,
                new BoolToVisibilityConverter(FalseToVisibility.Hidden)
                .Convert(new ExplicitCastableToBoolean(false), typeof(Visibility), null, CultureInfo.CurrentCulture));
        }

        public sealed class ImplicitCastableToBoolean
        {
            private bool value;

            public ImplicitCastableToBoolean(bool value = false)
            {
                this.value = value;
            }

            public static implicit operator bool(ImplicitCastableToBoolean obj)
            {
                return obj.value;
            }
        }

        public sealed class ExplicitCastableToBoolean
        {
            private bool value;

            public ExplicitCastableToBoolean(bool value = false)
            {
                this.value = value;
            }

            public static implicit operator bool(ExplicitCastableToBoolean obj)
            {
                return obj.value;
            }
        }
    }
}
