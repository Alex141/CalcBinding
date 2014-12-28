using CalcBinding;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using WpfExample;

namespace Tests
{
    [TestClass]
    public class ConverterTests
    {
        [TestMethod]
        public void ConvertToObjectTest()
        {
            var converter = new CalcConverter();

            Assert.AreEqual(15, converter.Convert(10, typeof(object), "{0}+5", CultureInfo.CurrentCulture));
            Assert.AreEqual(15.3, converter.Convert(10, typeof(object), "{0}+5.3", CultureInfo.CurrentCulture));
            Assert.AreEqual(true, converter.Convert(true, typeof(object), "{0}&&true", CultureInfo.CurrentCulture));
        }

        [TestMethod]
        public void ConvertToStringTest()
        {
            var converter = new CalcConverter();

            Assert.AreEqual("15", converter.Convert(10, typeof(string), "{0}+5", CultureInfo.CurrentCulture));
            Assert.AreEqual("15.3", converter.Convert(10, typeof(string), "{0}+5.3", CultureInfo.CurrentCulture));
            Assert.AreEqual("false", converter.Convert(false, typeof(string), "{0}&&true", CultureInfo.CurrentCulture));
        }

        [TestMethod]
        public void ConvertFromObjectTest()
        {
            var converter = new CalcConverter();

            Assert.AreEqual(10, converter.ConvertBack((object)15, typeof(int), "{0}+5", CultureInfo.CurrentCulture));
            Assert.AreEqual(10.4, converter.ConvertBack((object)15.0, typeof(double), "{0}+5.4", CultureInfo.CurrentCulture));
            Assert.AreEqual(true, converter.ConvertBack((object)false, typeof(bool), "!{0}", CultureInfo.CurrentCulture));
        }

        [TestMethod]
        public void ConvertFromStringTest()
        {
            var converter = new CalcConverter();

            Assert.AreEqual(10, converter.Convert("15", typeof(int), "{0}+5", CultureInfo.CurrentCulture));
            Assert.AreEqual(10.3, converter.Convert("15.7", typeof(double), "{0}+5.4", CultureInfo.CurrentCulture));
            Assert.AreEqual(false, converter.Convert("True", typeof(bool), "!{0}", CultureInfo.CurrentCulture));
        }
    }
}
