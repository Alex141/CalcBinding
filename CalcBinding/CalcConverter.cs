using NCalc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CalcBinding
{
    public class CalcConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //MSScriptControl.ScriptControl sc = new MSScriptControl.ScriptControl();
            //sc.Language = "VBScript";

            var expressionStr = (String)parameter;
            for (var i = 0; i < values.Count(); i++)
            {
                expressionStr = expressionStr.Replace("{" + i.ToString() + "}", values[i].ToString());
            }

            var expression = new Expression(expressionStr);
            object result = expression.Evaluate();

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
