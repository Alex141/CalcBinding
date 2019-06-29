using CalcBinding.Trace;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;

namespace Tests
{
    [TestClass]
    public sealed class TraceTests
    {
        [TestMethod]
        public void NoTracesFromCalcbindingByDefaultTest()
        {
            using (var sw = new StringWriter())
            {
                var listener = new TextWriterTraceListener(sw);

                try
                {
                    Tracer.Listeners.Add(listener);

                    var tracer = new Tracer(TraceComponent.CalcConverter);

                    tracer.TraceInformation("some info string");
                    tracer.TraceError("some error string");
                    tracer.TraceError("some debug string");
                }
                finally
                {
                    Trace.Listeners.Remove(listener);
                }

                var logs = sw.GetStringBuilder().ToString();

                Assert.IsTrue(
                    string.IsNullOrEmpty(logs),
                    $"There is logs from CalcBinding: \"{logs}\"");
            }
        }
    }
}
