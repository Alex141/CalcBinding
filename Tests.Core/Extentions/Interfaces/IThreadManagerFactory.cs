// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.TestFx.STAExtensions.Interfaces
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal interface IThreadManagerFactory
    {
        IThreadManager<TestResult[]> STAThreadManager { get; }
    }
}

