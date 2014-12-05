using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Linq.Expressions;
using DynamicExpresso;

namespace CalcBinding
{
    /// <summary>
    /// Converter that supports expression evaluate
    /// </summary>
    public class CalcConverter : IValueConverter, IMultiValueConverter
    {
        #region IValueConverter
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(new [] { value }, targetType, parameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // parameter is expression template where variables replaced by {variable number}
            var exprTemplate = (String)parameter;

            if (exprTemplate == "!{0}" || exprTemplate == "{0}" && targetType == typeof(bool))
            {
                if (value.GetType() == typeof(Visibility))
                {
                    value = new BoolToVisibilityConverter(FalseToVisibility)
                        .ConvertBack(value, targetType, null, culture);
                }

                if (value.GetType() == typeof(bool))
                    if (exprTemplate == "!{0}")
                        return !(bool)value;
                    else
                        return (bool)value;
            }

            throw new NotSupportedException();
        } 

        #endregion
        
        #region IMultiValueConverter
        
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (compiledExpression == null)
            {
                var expressionStr = (String)parameter;

                for (int i = 0; i < values.Count(); i++)
                {
                    expressionStr = expressionStr.Replace("{" + i.ToString() + "}", getVariableName(i));
                }

                var parametersDefinition = new List<Parameter>();

                // we can't determine value type if value is null
                if (values.Any(v => v == null))
                    return null;

                for (var i = 0; i < values.Count(); i++)
                {
                    parametersDefinition.Add(
                        new Parameter(getVariableName(i), values[i].GetType()));
                }

                compiledExpression = new Interpreter().Parse(expressionStr, parametersDefinition.ToArray());
            }

            var result = compiledExpression.Invoke(values);

            if (!StringFormatDefined)
            {
                if (targetType == typeof(Visibility))
                {
                    result = new BoolToVisibilityConverter(FalseToVisibility)
                                    .Convert(result, targetType, null, culture);
                }

                if (targetType == typeof(String))
                    result = result.ToString();
            }

            return result;
        }

        /// <summary>
        /// Returns string of one char, following from 'a' on i positions (1 -> b, 2 -> c)
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private string getVariableName(int i)
        {
            return new string( new[] { (Char)(i + (int)'a') });
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        } 

        #endregion

        public FalseToVisibility FalseToVisibility { get; set; }
        private Lambda compiledExpression;
        public bool StringFormatDefined { get; set; }

        #region Init
        
        public CalcConverter()
        {
            FalseToVisibility = CalcBinding.FalseToVisibility.Collapsed;
        }

        public CalcConverter(FalseToVisibility falseToVisibility)
        {
            FalseToVisibility = falseToVisibility;
        } 

        #endregion
    }
}
