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
using Tests.Mocks;

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
            
            converter = new CalcConverter();
            Assert.AreEqual(15.3, converter.Convert(10, typeof(object), "{0}+5.3", CultureInfo.CurrentCulture));
            
            converter = new CalcConverter();
            Assert.AreEqual(true, converter.Convert(true, typeof(object), "{0}&&true", CultureInfo.CurrentCulture));
        }

        [TestMethod]
        public void ConvertToStringTest()
        {
            var converter = new CalcConverter();
            Assert.AreEqual("15", converter.Convert(10, typeof(string), "{0}+(double)5", CultureInfo.CurrentCulture));

            converter = new CalcConverter();
            Assert.AreEqual("15.3", converter.Convert(10, typeof(string), "{0}+5.3", CultureInfo.CurrentCulture));

            converter = new CalcConverter();
            Assert.AreEqual("False", converter.Convert(false, typeof(string), "{0}&&true", CultureInfo.CurrentCulture));
        }

        [TestMethod]
        public void ConvertFromObjectTest()
        {
            var converter = new CalcConverter();
            Assert.AreEqual(10, converter.ConvertBack((object)15, typeof(int), "{0}+5", CultureInfo.CurrentCulture));
            
            converter = new CalcConverter();
            Assert.AreEqual(9.2, converter.ConvertBack((object)15.0, typeof(double), "{0}+5.8", CultureInfo.CurrentCulture));
            
            converter = new CalcConverter();
            Assert.AreEqual(true, converter.ConvertBack((object)false, typeof(bool), "!{0}", CultureInfo.CurrentCulture));
        }

        [TestMethod]
        public void ConvertFromStringTest()
        {
            var converter = new CalcConverter();
            Assert.AreEqual(10, converter.ConvertBack("15", typeof(int), "{0}+5", CultureInfo.CurrentCulture));

            converter = new CalcConverter();
            Assert.AreEqual(10.299999999999999, converter.ConvertBack("15.7", typeof(double), "{0}+5.4", CultureInfo.CurrentCulture));

            converter = new CalcConverter();
            Assert.AreEqual(false, converter.ConvertBack("True", typeof(bool), "!{0}", CultureInfo.CurrentCulture));
        }

        [TestMethod]
        public void BindingCompileTimesShouldBeOneTest()
        {
            var interpreterMock = new CompileTimesMockInterpreterParser();
            var converter = new CalcConverter(interpreterMock);

            converter.Convert("15", typeof(string), "{0}+\"5\"", CultureInfo.CurrentCulture);

            Assert.AreEqual(1, interpreterMock.ParseCalls.Count);
            Assert.AreEqual(1, interpreterMock.ParseCalls.First().Value);
        }

        [TestMethod]
        public void BindingBackCompileTimesShouldBeTwoTest()
        {
            var interpreterMock = new CompileTimesMockInterpreterParser();
            var converter = new CalcConverter(interpreterMock);

            converter.ConvertBack("15", typeof(int), "{0}+5", CultureInfo.CurrentCulture);

            Assert.AreEqual(2, interpreterMock.ParseCalls.Count);
            Assert.AreEqual(1, interpreterMock.ParseCalls.First().Value);
            Assert.AreEqual(1, interpreterMock.ParseCalls.Skip(1).First().Value);
        }

        [TestMethod]
        public void BindingBadTest()
        {
            new CalcConverter().Convert(null, typeof(string), "{0}+3", CultureInfo.InvariantCulture);
            new CalcConverter().Convert(DependencyProperty.UnsetValue, typeof(string), "{0}+3", CultureInfo.InvariantCulture);
            new CalcConverter().Convert(new object[] { 2, null }, typeof(string), "{0}+3", CultureInfo.InvariantCulture);
            new CalcConverter().Convert(new object[] { 2, DependencyProperty.UnsetValue }, typeof(string), "{0}+3", CultureInfo.InvariantCulture);
            new CalcConverter().Convert("asdf", typeof(int), "{1}*3", CultureInfo.InvariantCulture);

            new CalcConverter().ConvertBack(null, typeof(int), "{0}+3", CultureInfo.InvariantCulture);
            new CalcConverter().ConvertBack(DependencyProperty.UnsetValue, typeof(int), "{0}+3", CultureInfo.InvariantCulture);
            new CalcConverter().ConvertBack("sfdf", typeof(int), "{0}+3", CultureInfo.InvariantCulture);
        }
    }
}
