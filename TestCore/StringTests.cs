using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfExample;

namespace Tests
{
    [TestClass]
    public class StringTests:BaseSystemTests
    {
        #region SingleQuotes disabled

        [Fact]
        public void DoubleQuotesStringTest()
        {
            var binding = new CalcBinding.Binding("local:StaticExampleClass.StaticString+\"1224\"");

            StringAndObjectBindingAssert(binding, null,
                () => StaticExampleClass.StaticString = "0'-1", "0'-11224", "0'-11224",
                () => StaticExampleClass.StaticString = "'G", "'G1224", "'G1224",
                new Dictionary<string, Type>() { { "local:StaticExampleClass", typeof(StaticExampleClass) } }
            );
        }

        [Fact]
        public void SingleQuotesStringTest()
        {
            var binding = new CalcBinding.Binding("local:StaticExampleClass.StaticString+'1224'");

            StringAndObjectBindingAssert(binding, null,
                () => StaticExampleClass.StaticString = "0'-1", "0'-11224", "0'-11224",
                () => StaticExampleClass.StaticString = "'G", "'G1224", "'G1224",
                new Dictionary<string, Type>() { { "local:StaticExampleClass", typeof(StaticExampleClass) } }
            );
        }

        [Fact]
        public void CharWithoutSingleQuotesModeErrorTest()
        {
            var source = new ExampleViewModel();

            var binding = new CalcBinding.Binding("(Symbol == 'a' ? \"12'24\" : \"BC\"");

            StringAndObjectBindingAssert(binding, source,
                () => source.Symbol = 'a', "", null,
                () => source.Symbol = '1', "", null
            );
        }

        [Fact]
        public void StringWithSingleQuoteErrorTest()
        {
            var binding = new CalcBinding.Binding("local:StaticExampleClass.StaticString+\"12'2\"");

            StringAndObjectBindingAssert(binding, null,
                () => StaticExampleClass.StaticString = "0'-1'", "", null,
                () => StaticExampleClass.StaticString = "'G", "", null,
                new Dictionary<string, Type>() { { "local:StaticExampleClass", typeof(StaticExampleClass) } }
            );
        }

        #endregion    

        #region SingleQuotes enabled

        [Fact]
        public void CharWithSingleQuotesWithSingleQuotesModeTest()
        {
            var source = new ExampleViewModel();

            var binding = new CalcBinding.Binding("Symbol == 'a' ? 1224 : 24");
            binding.SingleQuotes = true;

            StringAndObjectBindingAssert(binding, source,
                () => source.Symbol = 'a', "1224", 1224,
                () => source.Symbol = '1', "24", 24
            );
        }

        [Fact]
        public void CharWithDoubleQuotesWithSingleQuotesModeTest()
        {
            var source = new ExampleViewModel();

            var binding = new CalcBinding.Binding("Symbol == \"2\" ? 1224 : 24");
            binding.SingleQuotes = true;

            StringAndObjectBindingAssert(binding, source,
                () => source.Symbol = '1', "24", 24,
                () => source.Symbol = '2', "1224", 1224
            );
        }

        [Fact]
        public void SingleQuotesStringErrorTest()
        {
            var binding = new CalcBinding.Binding("local:StaticExampleClass.StaticString+'1224'");
            binding.SingleQuotes = true;

            StringAndObjectBindingAssert(binding, null,
                () => StaticExampleClass.StaticString = "0'-1", "", null,
                () => StaticExampleClass.StaticString = "'G", "", null,
                new Dictionary<string, Type>() { { "local:StaticExampleClass", typeof(StaticExampleClass) } }
            );
        }

        [Fact]
        public void DoubleQuotesStringErrorTest()
        {
            var binding = new CalcBinding.Binding("local:StaticExampleClass.StaticString+\"1224\"");
            binding.SingleQuotes = true;

            StringAndObjectBindingAssert(binding, null,
                () => StaticExampleClass.StaticString = "0'-1", "", null,
                () => StaticExampleClass.StaticString = "'G", "", null,
                new Dictionary<string, Type>() { { "local:StaticExampleClass", typeof(StaticExampleClass) } }
            );
        }
        #endregion
    }
}
