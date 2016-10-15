using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WpfExample;
using System.Collections.Generic;

namespace Tests
{
    [TestClass]
    public class StaticPropertiesTests:BaseSystemTests
    {
        [TestMethod]
        public void SingleStaticPropertyWithBracketsTest()
        {  
            StringAndObjectBindingAssert("(local:StaticExampleClass.StaticA)", null,
                () => StaticExampleClass.StaticA = 10, "10", (double)10,
                () => StaticExampleClass.StaticA = 20.34, "20.34", 20.34,
                new Dictionary<string, Type>() { {"local:StaticExampleClass", typeof(StaticExampleClass) }}
            );
        }

        [TestMethod]
        public void SingleStaticPropertyWithoutBracketsTest()
        {
            StringAndObjectBindingAssert("local:StaticExampleClass.StaticA", null,
                () => StaticExampleClass.StaticA = 10, "10", (double)10,
                () => StaticExampleClass.StaticA = 20.34, "20.34", 20.34,
                new Dictionary<string, Type>() { { "local:StaticExampleClass", typeof(StaticExampleClass) } }
            );
        }

        //todo: test for binding where Path property sets instead of transfer path as string to constructor

        [TestMethod]
        public void MathStaticPropertyTest()
        {
            StringAndObjectBindingAssert("local:StaticExampleClass.StaticA+5", null,
                () => StaticExampleClass.StaticA = 10, "15", (double)15,
                () => StaticExampleClass.StaticA = 20.34, "25.34", 25.34,
                new Dictionary<string, Type>() { { "local:StaticExampleClass", typeof(StaticExampleClass) } }
            );
        }

        // test for error when set binding to static property and source automatically??

        //test: visibility binds to static property bool (bug with (n*{1})n* recognition)

        //readonly static property

        //static property without event (such as Brush) (peharps binding with flag oneWay)
    }
}
