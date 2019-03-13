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
using System.Windows.Media;
using Tests.Mocks;

namespace Tests
{
#if NETCOREAPP3_0
    [Microsoft.VisualStudio.TestTools.UnitTesting.STAExtensions.STATestClass]
#else 
    [TestClass]
#endif
    public class PropertyPathAnalyzerTests
    {
        [TestMethod]
        public void KnownDelimitersTest()
        {
            // we need to fix known delimeters array, as it binds to TokenType

            CollectionAssert.AreEquivalent(new char[] { '.', ':' }, PropertyPathAnalyzer.KnownDelimiters);

            //":" - for TokenType.Colon, "." - for TokenType.Dot
        }

        [TestMethod]
        public void PropertyPathTokensTest()
        {
            AssertPropertyPathes("MyProp.N+Other+(Next.N5ext1._Next2)-MyProp+4.56-MyProp.N", null, true,
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
            AssertPropertyPathes("local:MyClass.MyProp.N^local:MyClass.Other.B/(local:MyClass.Next.Next1.Next24)*(4.34*local:MyClass.MyProp.N)+local:MyClass.MyProp.M-24", null, true,
                new StaticPropertyPathToken(0, 21, "local", "MyClass", new[] { "MyProp", "N" }),
                new StaticPropertyPathToken(23, 43, "local", "MyClass", new[] { "Other", "B" }),
                new StaticPropertyPathToken(46, 76, "local", "MyClass", new[] { "Next", "Next1", "Next24" }),
                new StaticPropertyPathToken(85, 106, "local", "MyClass", new[] { "MyProp", "N" }),
                new StaticPropertyPathToken(109, 130, "local", "MyClass", new[] { "MyProp", "M" })
            );
        }

        [TestMethod]
        public void EnumPropertyPathTokensTest()
        {
            var resolver = new XamlTypeResolverMock(new Dictionary<string, Type>()
            {
                {"local:Enum1", typeof(ExampleEnum1)},
                {"local:Enum2", typeof(ExampleEnum2)},
            });

            AssertPropertyPathes("(1 > 0) ? local:MyClass.MyProp : local:MyClass.Prop", resolver, true,
                new StaticPropertyPathToken(10, 29, "local", "MyClass", new[] { "MyProp" }),
                new StaticPropertyPathToken(33, 50, "local", "MyClass", new[] { "Prop" })
            );
        }

        [TestMethod]
        public void ComplexTernarOperatorTest()
        {
            var resolver = new XamlTypeResolverMock(new Dictionary<string,Type>()
            {
                {"local:Enum1", typeof(ExampleEnum1)},
                {"local:Enum2", typeof(ExampleEnum2)},
            });

            AssertPropertyPathes("(1 > 0) ? local:MyClass.MyProp : local:MyClass.Prop", resolver, true,
                new StaticPropertyPathToken(10, 29, "local", "MyClass", new[] {"MyProp"}),
                new StaticPropertyPathToken(33, 50, "local", "MyClass", new[] {"Prop"})
            );

            AssertPropertyPathes("1 > 0 ? MyProp : local:MyClass.Prop", resolver, true,
                new PropertyPathToken(8, 13, new[] {"MyProp"}),
                new StaticPropertyPathToken(17, 34, "local", "MyClass", new[] {"Prop"})
            );

            AssertPropertyPathes("1 > 0 ? Local : Prop", resolver, true,
                new PropertyPathToken(8, 12, new[] {"Local"}),
                new PropertyPathToken(16, 19, new[] {"Prop"})
            );
          
            AssertPropertyPathes("1 > 0 ? Local:Prop.P1 : P2", resolver, true,
                new StaticPropertyPathToken(8, 20, "Local", "Prop", new[] {"P1"}),
                new PropertyPathToken(24, 25, new [] {"P2"})
            );

            AssertPropertyPathes("1 > 0 ? (5+Local:Prop.P1): P2", resolver, true,
                new StaticPropertyPathToken(11, 23, "Local", "Prop", new[] {"P1"}),
                new PropertyPathToken(27, 28, new[] {"P2"})
            );

            AssertPropertyPathes("1 > 0 ? (\"5\"+Local:Prop.P1): P2", resolver, true,
                new StaticPropertyPathToken(13, 25, "Local", "Prop", new[] { "P1" }),
                new PropertyPathToken(29, 30, new[] { "P2" })
            );
            
            AssertPropertyPathes("1 > 0 ? (Local1:Class2.P4+Local:Prop.P1) : P2", resolver, true,
                new StaticPropertyPathToken(9, 24, "Local1", "Class2", new[] {"P4"}),
                new StaticPropertyPathToken(26, 38, "Local", "Prop", new[] {"P1"}),
                new PropertyPathToken(43, 44, new[] {"P2"})
            ); 

            AssertPropertyPathes("1 > 0 ? local:Enum1.Prop1 : local:Enum2.Prop2", resolver, true,
                new EnumToken(8, 24, "local", typeof(ExampleEnum1), "Prop1"),
                new EnumToken(28, 44, "local", typeof(ExampleEnum2), "Prop2")
            );
        }

        [TestMethod]
        public void ColorsTest()
        {
            var colorsResolver = new XamlTypeResolverMock(new Dictionary<string,Type>{{"colors:Colors", typeof(Colors)}});
            AssertPropertyPathes("Enabled?colors:Colors.White : colors:Colors.Black", colorsResolver, false,
                new PropertyPathToken(0, 0, new []{"Enabled"}),
                new StaticPropertyPathToken(0, 0, "colors", "Colors", new [] {"White"}),
                new StaticPropertyPathToken(0, 0, "colors", "Colors", new[] { "Black" }));
        }

        [TestMethod]
        public void BadUseOfTernarOperator()
        {
            var analyzer = new PropertyPathAnalyzer();
            var resolver = new XamlTypeResolverMock(null);

            var badPathes = new []{
                "(Color>local:Colors.Enum?Color:Color",
                "(Color>local:Colors.Enum?Color: Color",
                "(Color>local:Colors.Enum?Color :Color",
                "(Color>local:Colors.Enum?local:Color.prop: Color",
                "(Color>local:Colors.Enum?local:Color.prop :Color",
            };

            foreach(var path in badPathes)
            {
                try
                {
                    analyzer.GetPathes(path, null);
                }
                catch (Exception e)
                {
                    continue;
                }
                Assert.Fail("Exception must be thrown! Path = {0}", path);
            }
        }

        [TestMethod]
        public void MathPathTokensTest()
        {
            AssertPropertyPathes("Math.PI+Math.Sin(45.5)*Math.Cos(Math.ATan(55))", null, true,
                            new MathToken(0, 6, "PI"),
                            new MathToken(8, 15, "Sin"),
                            new MathToken(23, 30, "Cos"),
                            new MathToken(32, 40, "ATan")
                        );
        }

        [TestMethod]
        public void InternatializationPathTokensTest()
        {
            var emptyResolver = new XamlTypeResolverMock(null);

            AssertPropertyPathes("1 > 0 ? local:propiedad.icône : local:Класс.中國.český", emptyResolver, true,
                            new StaticPropertyPathToken(8, 28, "local", "propiedad", new[] { "icône" }),
                            new StaticPropertyPathToken(32, 51, "local", "Класс", new[] { "中國", "český" })
                        );
        }

        [TestMethod]
        public void ParsingPathWithStringConstantsTest()
        {
            var emptyResolver = new XamlTypeResolverMock(null);

            AssertPropertyPathes("S + \"1\" + \"0\"", emptyResolver, true,
                            new PropertyPathToken(0, 0, new[] { "S" })
                        );

            AssertPropertyPathes("S + \"local:MyClass.Property\" + \"0\"", emptyResolver, true,
                              new PropertyPathToken(0, 0, new[] { "S" })
                          );
   
            AssertPropertyPathes("S + \"'local:MyClass.Property'\" + \"0\"", emptyResolver, true,
                                 new PropertyPathToken(0, 0, new[] { "S" })
                             );

            AssertPropertyPathes("S + \"'local:MyClass.Property\" + \"0\"", emptyResolver, true,
                                 new PropertyPathToken(0, 0, new[] { "S" })
                             );

            AssertPropertyPathes("S + \"\" + \"0\"", emptyResolver, true,
                                 new PropertyPathToken(0, 0, new[] { "S" })
                             );      
            //"5:5", "local:MyProp.Nested", "'sdfsdf'", "'", "", '' more more..
        }

        [TestMethod]
        public void ParsingPathWithCharConstantsTest()
        {
            var emptyResolver = new XamlTypeResolverMock(null);

            AssertPropertyPathes("S + '1' + '0'", emptyResolver, true,
                            new PropertyPathToken(0, 0, new[] { "S" })
                        );

            AssertPropertyPathes("S + ''", emptyResolver, true,
                            new PropertyPathToken(0, 0, new[] { "S" })
                        );

            AssertPropertyPathes("S + '\"'", emptyResolver, true,
                            new PropertyPathToken(0, 0, new[] { "S" })
                        );
        }

        private void AssertPropertyPathes(string path, IXamlTypeResolver typeResolver, bool positionsCheck, params PathToken[] expectedTokens)
        {
            var analyzer = new PropertyPathAnalyzer();

            var tokens = analyzer.GetPathes(path, typeResolver);

            CollectionAssert.AreEqual(expectedTokens.ToList(), tokens, new TestPathComparer(positionsCheck));
        }
    }

    public enum ExampleEnum1
    {
    }

    public enum ExampleEnum2
    {
        Value1
    }
}
