using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests 
{
    public class ExpectedExceptionExAttribute:ExpectedExceptionBaseAttribute
    {
        public int HResult { get; set; }
        public Type ExceptionType { get; private set; }

        public ExpectedExceptionExAttribute(Type exceptionType)
        {
            ExceptionType = exceptionType;
            HResult = 0;
        }

        public ExpectedExceptionExAttribute(Type exceptionType, int hResult):this(exceptionType)
        {
            HResult = hResult;
        }

        protected override void Verify(Exception exception)
        {
            var differentExceptionTypesMsgTemplate = "Current test method threw exception {1}, but exception {2} was expected. Exception message: {3}: {4}";
            var differentExceptionHResultsMsgTemplate = "Current test method threw exception {1} with HResult {2}, but exception {3} with HResult {4} was expected. Exception message: {5}: {6}";
            
            if (exception.GetType() != ExceptionType)
            {
                var msg = String.Format(differentExceptionTypesMsgTemplate, "testname", exception.GetType(), ExceptionType, exception.GetType(), exception.Message);
                throw new Exception(msg);
            }

            if (HResult != 0 && exception.HResult != HResult)
            {
                var msg = String.Format(differentExceptionHResultsMsgTemplate, "testname", exception.GetType(), exception.HResult, ExceptionType, HResult, exception.GetType(), exception.Message);
                throw new Exception(msg);
            }
        }
    }
}
