using CalcBinding;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
