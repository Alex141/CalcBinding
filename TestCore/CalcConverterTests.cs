using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using System.Linq;
using System.Windows;
using Tests.Mocks;

namespace Tests
{
    [TestClass]
    public class ConverterTests : BaseUnitTests
    {
        [Fact]
        public void ConvertToObjectTest()
        {
            var converter = CreateConverter();
            Assert.AreEqual(15, converter.Convert(10, typeof(object), "{0}+5", CultureInfo.CurrentCulture));
            
            converter = CreateConverter();
            Assert.AreEqual(15.3, converter.Convert(10, typeof(object), "{0}+5.3", CultureInfo.CurrentCulture));
            
            converter = CreateConverter();
            Assert.AreEqual(true, converter.Convert(true, typeof(object), "{0}&&true", CultureInfo.CurrentCulture));
        }

        [Fact]
        public void ConvertToStringTest()
        {
            var converter = CreateConverter();
            Assert.AreEqual("15", converter.Convert(10, typeof(string), "{0}+(double)5", CultureInfo.CurrentCulture));

            converter = CreateConverter();
            Assert.AreEqual("15.3", converter.Convert(10, typeof(string), "{0}+5.3", CultureInfo.CurrentCulture));

            converter = CreateConverter();
            Assert.AreEqual("False", converter.Convert(false, typeof(string), "{0}&&true", CultureInfo.CurrentCulture));
        }

        [Fact]
        public void ConvertFromObjectTest()
        {
            var converter = CreateConverter();
            Assert.AreEqual(10, converter.ConvertBack((object)15, typeof(int), "{0}+5", CultureInfo.CurrentCulture));

            converter = CreateConverter();
            Assert.AreEqual(9.2, converter.ConvertBack((object)15.0, typeof(double), "{0}+5.8", CultureInfo.CurrentCulture));

            converter = CreateConverter();
            Assert.AreEqual(true, converter.ConvertBack((object)false, typeof(bool), "!{0}", CultureInfo.CurrentCulture));
        }

        [Fact]
        public void NullableValueTypesAreNotSupportedInConvertBackBinding()
        {
            var converter = CreateConverter();
            // instead of NULL there must be //5// but nullable values doesn't support so binding retrieve NULL instead of //5//
            Assert.AreEqual(null, converter.ConvertBack((object)10.0, typeof(double?), "{0}+5", CultureInfo.CurrentCulture));        
        }

        [Fact]
        public void ConvertFromStringTest()
        {
            var converter = CreateConverter();
            Assert.AreEqual(10, converter.ConvertBack("15", typeof(int), "{0}+5", CultureInfo.CurrentCulture));

            converter = CreateConverter();
            Assert.AreEqual(10.299999999999999, converter.ConvertBack("15.7", typeof(double), "{0}+5.4", CultureInfo.CurrentCulture));

            converter = CreateConverter();
            Assert.AreEqual(false, converter.ConvertBack("True", typeof(bool), "!{0}", CultureInfo.CurrentCulture));
        }

        [Fact]
        public void BindingCompileTimesShouldBeOneTest()
        {
            var interpreterMock = new CompileTimesMockInterpreterParser();
            var converter = CreateConverter(interpreterMock);

            converter.Convert("15", typeof(string), "{0}+\"5\"", CultureInfo.CurrentCulture);

            Assert.AreEqual(1, interpreterMock.ParseCallsByExpressions.Count);
            Assert.AreEqual(1, interpreterMock.ParseCallsByExpressions.First().Value);
        }

        [Fact]
        public void BindingBackCompileTimesShouldBeTwoTest()
        {
            var interpreterMock = new CompileTimesMockInterpreterParser();
            var converter = CreateConverter(interpreterMock);

            converter.ConvertBack("15", typeof(int), "{0}+5", CultureInfo.CurrentCulture);

            Assert.AreEqual(2, interpreterMock.ParseCallsByExpressions.Count);
            Assert.AreEqual(1, interpreterMock.ParseCallsByExpressions.First().Value);
            Assert.AreEqual(1, interpreterMock.ParseCallsByExpressions.Skip(1).First().Value);
        }

        [Fact]
        public void BindingBadTest()
        {
            CreateConverter().Convert(null, typeof(string), "{0}+3", CultureInfo.InvariantCulture);
            CreateConverter().Convert(DependencyProperty.UnsetValue, typeof(string), "{0}+3", CultureInfo.InvariantCulture);
            CreateConverter().Convert(new object[] { 2, null }, typeof(string), "{0}+3", CultureInfo.InvariantCulture);
            CreateConverter().Convert(new object[] { 2, DependencyProperty.UnsetValue }, typeof(string), "{0}+3", CultureInfo.InvariantCulture);
            CreateConverter().Convert("asdf", typeof(int), "{1}*3", CultureInfo.InvariantCulture);

            CreateConverter().ConvertBack(null, typeof(int), "{0}+3", CultureInfo.InvariantCulture);
            CreateConverter().ConvertBack(DependencyProperty.UnsetValue, typeof(int), "{0}+3", CultureInfo.InvariantCulture);
            CreateConverter().ConvertBack("sfdf", typeof(int), "{0}+3", CultureInfo.InvariantCulture);
        }
    }
}
