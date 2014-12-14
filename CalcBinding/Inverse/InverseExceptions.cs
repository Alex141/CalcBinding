using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalcBinding.Inverse
{
    [Serializable]
    public class InverseException : System.Exception
    {
        public InverseException() { }
        public InverseException(string message) : base(message) { }
        public InverseException(string message, System.Exception inner) : base(message, inner) { }
        protected InverseException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
