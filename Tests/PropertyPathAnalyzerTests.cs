using CalcBinding;
using CalcBinding.PathAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using Tests.Mocks;

namespace Tests
{
    [TestClass]
    public class PropertyPathAnalyzerTests
    {
        [TestMethod]
        public void KnownDelimitersTest()
        {
            // we need to fix known delimeters array, as it binds to TokenType

            CollectionAssert.AreEquivalent(new string[] { ".", ":" }, PropertyPathAnalyzer.KnownDelimiters);

            //":" - for TokenType.Colon, "." - for TokenType.Dot
        }

        [TestMethod]
        public void PropertyPathTokensTest()
        {
            AssertPropertyPathes("MyProp.N+Other+(Next.N5ext1._Next2)-MyProp+4.56-MyProp.N", null, 
                new PropertyPathToken(0, 7, new[] { "MyProp", "N" }),
                new PropertyPathToken(9, 13, new[] { "Other" }),
                new PropertyPathToken(16, 33, new[] { "Next", "N5ext1", "_Next2" }),
                new PropertyPathToken(36, 41, new[] { "MyProp" }),
                new PropertyPathToken(48, 55, new[] { "MyProp", "N" })
            );
        }

        [TestMethod]
        public void StaticPropertyPathTokensTest()
        {
            AssertPropertyPathes("local:MyClass.MyProp.N^local:MyClass.Other.B/(local:MyClass.Next.Next1.Next24)*(4.34*local:MyClass.MyProp.N)+local:MyClass.MyProp.M-24", null,
                new StaticPropertyPathToken(0, 21, "local", "MyClass", new[] { "MyProp", "N" }),
                new StaticPropertyPathToken(23, 43, "local", "MyClass", new[] { "MyProp", "Other", "B" }),
                new StaticPropertyPathToken(46, 78, "local", "MyClass", new[] { "MyProp", "Next", "Next1", "Next2" }),
                new StaticPropertyPathToken(85, 106, "local", "MyClass", new[] { "MyProp", "N" }),
                new StaticPropertyPathToken(109, 130, "local", "MyClass", new[] { "MyProp", "M" })
            );
        }

        [TestMethod]
        public void EnumPropertyPathTokensTest()
        {
            AssertPropertyPathes("(1 > 0) ? local:MyEnum : local:MyClass.P", null,
                new StaticPropertyPathToken(0, 21, "local", "MyClass", new[] { "MyProp", "N" }),
                new StaticPropertyPathToken(23, 43, "local", "MyClass", new[] { "MyProp", "Other", "B" }),
                new StaticPropertyPathToken(46, 78, "local", "MyClass", new[] { "MyProp", "Next", "Next1", "Next2" }),
                new StaticPropertyPathToken(85, 106, "local", "MyClass", new[] { "MyProp", "N" }),
                new StaticPropertyPathToken(109, 130, "local", "MyClass", new[] { "MyProp", "M" })
            );
        }

        [TestMethod]
        public void ComplexTernarOperatorTest()
        {
            var resolver = new XamlTypeResolverMock(new Dictionary<string,Type>()
            {
                {"local:Enum1", typeof(Enum1)},
                {"local:Enum2", typeof(Enum2)},
            });

            AssertPropertyPathes("(1 > 0) ? local:MyClass.MyProp : local:MyClass.Prop", resolver, 
                new StaticPropertyPathToken(10, 29, "local", "MyClass", new[] {"MyProp"}),
                new StaticPropertyPathToken(33, 50, "local", "MyClass", new[] {"Prop"})
            );

            AssertPropertyPathes("1 > 0 ? MyProp : local:MyClass.Prop", resolver, 
                new PropertyPathToken(8, 13, new[] {"MyProp"}),
                new StaticPropertyPathToken(17, 34, "local", "MyClass", new[] {"Prop"})
            );

            AssertPropertyPathes("1 > 0 ? Local : Prop", resolver, 
                new PropertyPathToken(8, 12, new[] {"Local"}),
                new PropertyPathToken(16, 19, new[] {"Prop"})
            );
          
            AssertPropertyPathes("1 > 0 ? Local:Prop.P1 : P2", resolver, 
                new StaticPropertyPathToken(8, 20, "Local", "Prop", new[] {"P1"}),
                new PropertyPathToken(24, 25, new [] {"P2"})
            );

            AssertPropertyPathes("1 > 0 ? (5+Local:Prop.P1): P2", resolver, 
                new StaticPropertyPathToken(11, 23, "Local", "Prop", new[] {"P1"}),
                new PropertyPathToken(27, 28, new[] {"P2"})
            );

            AssertPropertyPathes("1 > 0 ? (\"5\"+Local:Prop.P1): P2", resolver,
                new StaticPropertyPathToken(15, 27, "Local", "Prop", new[] { "P1" }),
                new PropertyPathToken(31, 32, new[] { "P2" })
            );
            
            AssertPropertyPathes("1 > 0 ? (Local1:Class2.P4+Local:Prop.P1) : P2", resolver, 
                new StaticPropertyPathToken(9, 24, "Local1", "Class2", new[] {"P4"}),
                new StaticPropertyPathToken(26, 38, "Local", "Prop", new[] {"P1"}),
                new PropertyPathToken(43, 44, new[] {"P2"})
            ); 

            AssertPropertyPathes("1 > 0 ? local:Enum1.Prop1 : local:Enum2.Prop2", resolver, 
                new EnumToken(8, 24, "local", typeof(Enum1), "Prop1"),
                new EnumToken(28, 44, "local", typeof(Enum2), "Prop2")
            );
        }

        [TestMethod]
        public void ColorsEnumTest()
        {
            throw new NotImplementedException();
            //Enabled?colors:Colors.White : colors:Colors.Black"

        }

        [TestMethod]
        public void MathPathTokensTest()
        {
        }

        [TestMethod]
        public void InternatializationPathTokensTest()
        {
        }

        [TestMethod]
        public void ParsingPathWithStringConstantsTest()
        {
            //"5:5", "local:MyProp.Nested", "'sdfsdf'"
        }

        private void AssertPropertyPathes(string path, IXamlTypeResolver typeResolver, params PathToken[] expectedTokens)
        {
            var analyzer = new PropertyPathAnalyzer();

            var tokens = analyzer.GetPathes(path, typeResolver);

            CollectionAssert.AreEqual(expectedTokens.ToList(), tokens, new TestPathComparer());
        }

    }

    public enum Enum1
    {
    }

    public enum Enum2
    {
    }
}
