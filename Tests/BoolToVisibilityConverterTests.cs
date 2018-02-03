using CalcBinding;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using System.Windows;

namespace Tests
{
    [TestClass]
    public class BoolToVisibilityConverterTests
    {
        [TestMethod]
        public void ConvertBoolToVisibilityTest()
        {
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
                .Convert(new CastableToBoolean(true), typeof(Visibility), null, CultureInfo.CurrentCulture));

            Assert.AreEqual(Visibility.Collapsed,
                new BoolToVisibilityConverter()
                .Convert(new CastableToBoolean(false), typeof(Visibility), null, CultureInfo.CurrentCulture));

            Assert.AreEqual(Visibility.Hidden,
                new BoolToVisibilityConverter(FalseToVisibility.Hidden)
                .Convert(new CastableToBoolean(false), typeof(Visibility), null, CultureInfo.CurrentCulture));
        }

        private sealed class CastableToBoolean
        {
            private bool value;

            public CastableToBoolean(bool value = false)
            {
                this.value = value;
            }

            public static implicit operator bool(CastableToBoolean obj)
            {
                return obj.value;
            }
        }
    }
}
