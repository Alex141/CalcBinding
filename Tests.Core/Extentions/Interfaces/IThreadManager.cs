// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.TestFx.STAExtensions.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal interface IThreadManager<T> : IDisposable
    {
        /// <summary>
        /// Executes the function in a thread
        /// </summary>
        /// <param name="functionToExecuteOnThread">Function to execute</param>
        /// <returns>T: value returned by function</returns>
        T Execute(Func<T> functionToExecuteOnThread);
    }
}
