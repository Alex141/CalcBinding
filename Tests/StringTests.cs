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
        #region DifferQuotes disabled

        [TestMethod]
        public void DoubleQuotesStringTest()
        {
            var binding = new CalcBinding.Binding("local:StaticExampleClass.StaticString+\"1224\"");

            StringAndObjectBindingAssert(binding, null,
                () => StaticExampleClass.StaticString = "0'-1", "0'-11224", "0'-11224",
                () => StaticExampleClass.StaticString = "'G", "'G1224", "'G1224",
                new Dictionary<string, Type>() { { "local:StaticExampleClass", typeof(StaticExampleClass) } }
            );
        }

        [TestMethod]
        public void SingleQuotesStringTest()
        {
            var binding = new CalcBinding.Binding("local:StaticExampleClass.StaticString+'1224'");

            StringAndObjectBindingAssert(binding, null,
                () => StaticExampleClass.StaticString = "0'-1", "0'-11224", "0'-11224",
                () => StaticExampleClass.StaticString = "'G", "'G1224", "'G1224",
                new Dictionary<string, Type>() { { "local:StaticExampleClass", typeof(StaticExampleClass) } }
            );
        }

        [TestMethod]
        public void CharWithoutDifferQuotesErrorTest()
        {
            var source = new ExampleViewModel();

            var binding = new CalcBinding.Binding("(Symbol == 'a' ? \"12'24\" : \"BC\"");

            StringAndObjectBindingAssert(binding, source,
                () => source.Symbol = 'a', "", null,
                () => source.Symbol = '1', "", null
            );
        }

        [TestMethod]
        public void StringWithDifferQuotesErrorTest()
        {
            var binding = new CalcBinding.Binding("local:StaticExampleClass.StaticString+\"12'2\"");

            StringAndObjectBindingAssert(binding, null,
                () => StaticExampleClass.StaticString = "0'-1'", "", null,
                () => StaticExampleClass.StaticString = "'G", "", null,
                new Dictionary<string, Type>() { { "local:StaticExampleClass", typeof(StaticExampleClass) } }
            );
        }

        #endregion    

        #region DifferQuotes enabled

        [TestMethod]
        public void CharWithDifferQuotesTest()
        {
            var source = new ExampleViewModel();

            var binding = new CalcBinding.Binding("Symbol == 'a' ? \"12'24\" : \"BC\"");
            binding.DifferQuotes = true;

            StringAndObjectBindingAssert(binding, source,
                () => source.Symbol = 'a', "12'24", "12'24",
                () => source.Symbol = '1', "BC", "BC"
            );
        }

        [TestMethod]
        public void ComplexCharWithDifferQuotesTest()
        {
            var source = new ExampleViewModel();

            var binding = new CalcBinding.Binding("Symbol == '\"' ? \"12'24\" : \"BC\"");
            binding.DifferQuotes = true;

            StringAndObjectBindingAssert(binding, source,
                () => source.Symbol = '1', "BC", "BC",
                () => source.Symbol = '\"', "12'24", "12'24"
            );
        }

        [TestMethod]
        public void StringWithDifferQuotesTest()
        {
            var binding = new CalcBinding.Binding("local:StaticExampleClass.StaticString+\"12'24 'local:model.A\"");
            binding.DifferQuotes = true;

            StringAndObjectBindingAssert(binding, null,
                () => StaticExampleClass.StaticString = "0'-1'", "0'-1'12'24 'local:model.A", "0'-1'12'24 'local:model.A",
                () => StaticExampleClass.StaticString = "'G", "'G12'24 'local:model.A", "'G12'24 'local:model.A",
                new Dictionary<string, Type>() { { "local:StaticExampleClass", typeof(StaticExampleClass) } }
            );
        }

        [TestMethod]
        public void SingleQuotesStringErrorTest()
        {
            var binding = new CalcBinding.Binding("local:StaticExampleClass.StaticString+'1224'");
            binding.DifferQuotes = true;

            StringAndObjectBindingAssert(binding, null,
                () => StaticExampleClass.StaticString = "0'-1", "", null,
                () => StaticExampleClass.StaticString = "'G", "", null,
                new Dictionary<string, Type>() { { "local:StaticExampleClass", typeof(StaticExampleClass) } }
            );
        }
        #endregion
    }
}
