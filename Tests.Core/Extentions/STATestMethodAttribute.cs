// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.VisualStudio.TestTools.UnitTesting.STAExtensions
{
    using Microsoft.TestFx.STAExtensions;
    using Microsoft.TestFx.STAExtensions.Interfaces;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines [STATestMethod] attribute which runs the specified test method under STAThread
    /// Note: Using this under a class with [STATestClass] Attribute is redundant.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class STATestMethodAttribute : TestMethodAttribute
    {
        private TestMethodAttribute actualTestMethodAttribute;

        private IThreadManagerFactory threadManagerFactory;

        /// <summary>
        /// Default constructor for reflection
        /// </summary>
        public STATestMethodAttribute() : this(null, ThreadManagerFactory.Instance)
        {
            // Default constructor for reflection - Type.GetCustomAttributes API
        }

        internal STATestMethodAttribute(TestMethodAttribute actualTestMethodAttribute, IThreadManagerFactory threadManagerFactory)
        {
            // Store the actual test method attr
            this.actualTestMethodAttribute = actualTestMethodAttribute;
            this.threadManagerFactory = threadManagerFactory;
        }

        /// <summary>
        /// Executes the ITestMethod in a STA Thread
        /// </summary>
        /// <param name="testMethod">TestMethod to invoke</param>
        /// <returns>TestResult of the execution</returns>
        public override TestResult[] Execute(ITestMethod testMethod)
        {
            Func<TestResult[]> func = () =>
                (this.actualTestMethodAttribute != null
                // If user used a custom testmethod attribute use its impl to execute test method
                ? this.actualTestMethodAttribute.Execute(testMethod)
                // user used STATestMethod attribute directly but not on class
                : base.Execute(testMethod));

            return this.threadManagerFactory.STAThreadManager.Execute(func);
        }
    }
}
