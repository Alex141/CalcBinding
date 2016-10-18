using CalcBinding;
using CalcBinding.PathAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
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
            //AssertPropertyPathes("MyProp", null, new PropertyPathToken(0, 5, new [] {"MyProp"}));
            //AssertPropertyPathes("MyProp+5.4", null, new PropertyPathToken(0, 5, new[] { "MyProp" }));
            //AssertPropertyPathes("5+MyProp", null, new PropertyPathToken(0, 5, new[] { "MyProp" }));
            //AssertPropertyPathes("MyProp.Nested", null, new PropertyPathToken(0, 12, new[] { "MyProp", "Nested" }));
            //AssertPropertyPathes("MyProp.Nested4+10-2.6", null, new PropertyPathToken(0, 12, new[] { "MyProp", "Nested4" }));

            //AssertPropertyPathes("MyProp.Nested5.N1estedNeste5d+5", null, new PropertyPathToken(0, 28, new[] { "MyProp", "Nested5", "N1estedNeste5d" }));
            //AssertPropertyPathes("MyProp+MyOtherProp+2", null, 
            //    new PropertyPathToken(0, 5, new[] { "MyProp" }),
            //    new PropertyPathToken(7, 17, new[] { "MyOtherProp"})
            //    );
            AssertPropertyPathes("MyProp.N+Other+(Next.N5ext1._Next2)-MyProp+4.56-MyProp.N", null, 
                new PropertyPathToken(0, 7, new[] { "MyProp", "N" }),
                new PropertyPathToken(9, 15, new[] { "Other" }),
                new PropertyPathToken(18, 34, new[] { "Next", "N5ext1", "_Next2" }),
                new PropertyPathToken(37, 44, new[] { "MyProp", "N" }),
                new PropertyPathToken(51, 59, new[] { "MyProp", "_M" })
                );

        }

        [TestMethod]
        public void StaticPropertyPathTokensTest()
        {
            AssertPropertyPathes("local:MyClass.MyProp", null, new StaticPropertyPathToken(0, 19, "local", "MyClass", new[] { "MyProp" }));
            AssertPropertyPathes("local2:MyClass.MyProp+5", null, new StaticPropertyPathToken(0, 20, "local2", "MyClass", new[] { "MyProp" }));
            AssertPropertyPathes("5.0+local:MyClass.MyProp", null, new StaticPropertyPathToken(4, 23, "local", "MyClass", new[] { "MyProp" }));
            AssertPropertyPathes("l:MyClass.MyProp.Nested", null, new StaticPropertyPathToken(2, 22, "l", "MyClass", new[] { "MyProp", "Nested" }));
            AssertPropertyPathes("local:CL5.MyProp.Nested+10", null, new StaticPropertyPathToken(0, 20, "local", "MyClass", new[] { "MyProp" }));

            AssertPropertyPathes("name:class.MyProp.Nested.NestedNested+5", null, new StaticPropertyPathToken(0, 25, "name", "class", new[] { "MyProp", "Nested", "NestedNested5" }));
            AssertPropertyPathes("c1:c1.Prop+c2:CLASS.MyOtherProp^2", null,
                new StaticPropertyPathToken(0, 20, "c1", "c1", new[] { "Prop" }),
                new StaticPropertyPathToken(0, 20, "c2", "CLASS", new[] { "MyOtherProp" })
                );

            AssertPropertyPathes("local:MyClass.MyProp.N+local:MyClass.Other.B/(local:MyClass.Next.Next1.Next2)*local:MyClass.MyProp.N+local:MyClass.MyProp.M", null,
            new StaticPropertyPathToken(0, 20, "local", "MyClass", new[] { "MyProp", "N" }),
            new StaticPropertyPathToken(0, 20, "local", "MyClass", new[] { "MyProp", "Other", "B" }),
            new StaticPropertyPathToken(0, 20, "local", "MyClass", new[] { "MyProp", "Next", "Next1", "Next2" }),
            new StaticPropertyPathToken(0, 20, "local", "MyClass", new[] { "MyProp", "N" }),
            new StaticPropertyPathToken(0, 20, "local", "MyClass", new[] { "MyProp", "M" })
                );
        }

        [TestMethod]
        public void InternatializationPathTokensTest()
        {
        }

        [TestMethod]
        public void ParsingPathWithStringConstantsTest()
        {

        }

        private void AssertPropertyPathes(string path, IXamlTypeResolver typeResolver, params PathToken[] expectedTokens)
        {
            var analyzer = new PropertyPathAnalyzer();

            var tokens = analyzer.GetPathes(path, typeResolver);

            CollectionAssert.AreEqual(expectedTokens.ToList(), tokens);
        }

    }
}
