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
            AssertPropertyPathes("local:MyClass.MyProp.N+local:MyClass.Other.B/(local:MyClass.Next.Next1.Next2)*local:MyClass.MyProp.N+local:MyClass.MyProp.M", null,
            new StaticPropertyPathToken(0, 20, "local", "MyClass", new[] { "MyProp", "N" }),
            new StaticPropertyPathToken(0, 20, "local", "MyClass", new[] { "MyProp", "Other", "B" }),
            new StaticPropertyPathToken(0, 20, "local", "MyClass", new[] { "MyProp", "Next", "Next1", "Next2" }),
            new StaticPropertyPathToken(0, 20, "local", "MyClass", new[] { "MyProp", "N" }),
            new StaticPropertyPathToken(0, 20, "local", "MyClass", new[] { "MyProp", "M" })
                );
        }

        [TestMethod]
        public void EnumPropertyPathTokensTest()
        {
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

        }

        private void AssertPropertyPathes(string path, IXamlTypeResolver typeResolver, params PathToken[] expectedTokens)
        {
            var analyzer = new PropertyPathAnalyzer();

            var tokens = analyzer.GetPathes(path, typeResolver);

            CollectionAssert.AreEqual(expectedTokens.ToList(), tokens);
        }

    }
}
