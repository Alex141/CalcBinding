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
    public class CalcConverter : IValueConverter, IMultiValueConverter
    {
        #region IValueConverter
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(new [] { value }, targetType, parameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var exprTemplate = (String)parameter; 

            if (exprTemplate == "!{0}" && targetType == typeof(bool))
            {
                if (value.GetType() == typeof(Visibility))
                {
                    value = new BoolToVisibilityConverter(FalseToVisibility)
                        .ConvertBack(value, targetType, null, culture);
                }

                if (value.GetType() == typeof(bool))
                    return !(bool)value;
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
                    expressionStr = expressionStr.Replace("{" + i.ToString() + "}", new string(new[] { (Char)(i + (int)'a') }));
                }

                var parametersDefinition = new List<Parameter>();

                for (var i = 0; i < values.Count(); i++)
                {
                    parametersDefinition.Add(
                        new Parameter(new string(new[] { (Char)(i + (int)'a') }), values[i].GetType()));
                }

                compiledExpression = new Interpreter().Parse(expressionStr, parametersDefinition.ToArray());
            }

            var result = compiledExpression.Invoke(values);

            if (targetType == typeof(Visibility))
            {
                result = new BoolToVisibilityConverter(FalseToVisibility)
                                .Convert(result, targetType, null, culture);
            }

            if (targetType == typeof(String))
                result = result.ToString();

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        } 

        #endregion

        public FalseToVisibility FalseToVisibility { get; set; }

        Lambda compiledExpression;

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

    public class BoolToVisibilityConverter : IValueConverter
    {
        public BoolToVisibilityConverter()
        {
            FalseToVisibility = FalseToVisibility.Collapsed;
        }

        public BoolToVisibilityConverter(FalseToVisibility falseToVisibility)
        {
            FalseToVisibility = falseToVisibility;
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var b = (bool)value;

            if (b)
                return Visibility.Visible;

            if (FalseToVisibility == FalseToVisibility.Collapsed)
                return Visibility.Collapsed;

            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var visible = (Visibility)value;

            return visible == Visibility.Visible;
        }

        public FalseToVisibility FalseToVisibility { get; set; }

    }

    public enum FalseToVisibility
    {
        Hidden,
        Collapsed
    }
}
