// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.TestFx.STAExtensions
{
    using Microsoft.TestFx.STAExtensions.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    internal class STAThreadManager<T> : IThreadManager<T>
    {
        private Thread staThread;

        private AutoResetEvent actionAvailableWaithHandle = new AutoResetEvent(false);
        private AutoResetEvent runCompletedAvailableWaithHandle = new AutoResetEvent(false);

        private Func<T> functionToExecuteOnThread;

        private TaskCompletionSource<T> taskCompletionSource;

        private object lockObject = new object();

        private bool disposing = false;

        public T Execute(Func<T> functionToExecuteOnThread)
        {
            lock (lockObject)
            {
                // Ensure thread initialized
                EnsureThreadInitialized();

                // Initialize Thread-specific vars
                this.taskCompletionSource = new TaskCompletionSource<T>();
                this.functionToExecuteOnThread = functionToExecuteOnThread;

                // Send signal to sta thread to execute above function
                this.actionAvailableWaithHandle.Set();

                // Wait for result
                var task = this.taskCompletionSource.Task;
                try
                {
                    task.Wait();
                    return task.Result;
                }
                catch(AggregateException ex)
                {
                    if (ex.InnerException != null)
                    {
                        throw ex.InnerException;
                    }
                    else throw;
                }
            }
        }

        private void EnsureThreadInitialized()
        {
            if (this.staThread == null)
            {
                this.staThread = new Thread(ThreadLoop);
                this.staThread.IsBackground = true;
                this.staThread.SetApartmentState(ApartmentState.STA);
                this.staThread.Name = "testfxSTAExThread";
                this.staThread.Start();

                // MsTestV2 has no way of telling the extensions that test run is complete so that they can cleanup
                // So we need to rely on AppDomain unload event to clean up resources
                // Destructor is useless as it gets called after AppDomain.unload killed all our threads
                // If we let the unload, it will throw "AppDomainUnloadedException" if any threads are still alive at the time
                AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;
            }
        }

        private void CurrentDomain_DomainUnload(object sender, EventArgs e)
        {
            AppDomain.CurrentDomain.DomainUnload -= CurrentDomain_DomainUnload;
            Dispose();
        }

        public void Dispose()
        {
            if (!this.disposing)
            {
                this.disposing = true;
                if (this.staThread != null && this.staThread.IsAlive)
                {
                    this.runCompletedAvailableWaithHandle.Set();
                    // TODO: Better way to cleanup - 100ms is really arbitrary
                    if (!this.staThread.Join(100))
                    {
                        this.staThread.Abort();
                        this.staThread.Join();
                    }
                    this.staThread = null;
                }
            }
        }

        private void ThreadLoop()
        {
            var waitForActions = true;
            while (waitForActions)
            {
                var waitResult = WaitHandle.WaitAny(new WaitHandle[2] { actionAvailableWaithHandle, runCompletedAvailableWaithHandle });
                if (waitResult == 0)
                {
                    if (this.functionToExecuteOnThread != null)
                    {
                        try
                        {
                            this.taskCompletionSource?.SetResult(this.functionToExecuteOnThread.Invoke());
                        }
                        catch (Exception ex)
                        {
                            if(disposing)
                            {
                                if(ex is ThreadAbortException)
                                {
                                    Thread.ResetAbort();
                                }
                                // Disposing, just exit the thread cleanly
                                waitForActions = false;
                            }
                            else 
                            {
                                this.taskCompletionSource?.SetException(ex);
                            }
                        }
                    }
                }
                else
                {
                    waitForActions = false;
                }
            }
        }
    }
}
