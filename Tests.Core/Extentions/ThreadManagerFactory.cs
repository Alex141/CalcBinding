// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.TestFx.STAExtensions
{
    using Microsoft.TestFx.STAExtensions.Interfaces;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class ThreadManagerFactory : IThreadManagerFactory
    {
        private static IThreadManagerFactory instance;

        private ThreadManagerFactory()
        {
            // Singleton
        }

        public static IThreadManagerFactory Instance
        {
            get
            {
                if (instance == null) instance = new ThreadManagerFactory();
                return instance;
            }
        }

        private IThreadManager<TestResult[]> staThreadManagerInstance;

        public IThreadManager<TestResult[]>  STAThreadManager
        {
            get
            {
                if (staThreadManagerInstance == null)
                {
                    staThreadManagerInstance = (IThreadManager<TestResult[]>) new STAThreadManager<TestResult[]>();
                }
                return staThreadManagerInstance;
            }
        }
    }
}
