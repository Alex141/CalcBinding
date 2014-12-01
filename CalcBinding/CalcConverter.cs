using NCalc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CalcBinding
{
    public class CalcConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var expressionStr = (String)parameter;
            for (var i = 0; i < values.Count(); i++)
            {
                expressionStr = expressionStr.Replace("{" + i.ToString() + "}", values[i].ToString().ToLower());
            }

            var expression = new Expression(expressionStr);
            object result = expression.Evaluate();

            return result.ToString();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
