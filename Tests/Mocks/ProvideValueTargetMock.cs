using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Tests.Mocks
{
    public class ProvideValueTargetMock : IProvideValueTarget
    {
        public object TargetObject { get; set; }
        public object TargetProperty { get; set; }

        public ProvideValueTargetMock()
        {
        }
    }
}
