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
        public void SingleStaticPropertyTest()
        {  
            StringAndObjectBindingAssert("local:StaticExampleClass.A", null,
                () => StaticExampleClass.StaticA = 10, "10", (double)10,
                () => StaticExampleClass.StaticA = 20.34, "20.34", 20.34,
                new Dictionary<string, Type>() { {"local:StaticExampleClass", typeof(StaticExampleClass) }}
            );
        }
    }
}
