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
        private IExpressionParser parser = new InterpreterParser();
        private Lambda compiledExpression;
        private Lambda compiledInversedExpression;
        private bool inverseFaulted = false;

        public bool StringFormatDefined { get; set; }

        private FalseToVisibility falseToVisibility = CalcBinding.FalseToVisibility.Collapsed;
        public FalseToVisibility FalseToVisibility 
        {
            get { return falseToVisibility; }
            set { falseToVisibility = value; }
        }

        #region Init

        public CalcConverter() { }

        public CalcConverter(IExpressionParser parser)
        {
            this.parser = parser;        
        }

        public CalcConverter(FalseToVisibility falseToVisibility)
        {
            FalseToVisibility = falseToVisibility;
        }

        public CalcConverter(FalseToVisibility falseToVisibility, IExpressionParser parser)
        {
            FalseToVisibility = falseToVisibility;
            this.parser = parser;
        } 

        #endregion

        #region IValueConverter
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(new [] { value }, targetType, parameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (inverseFaulted)
                throw new Exception("bad"); //todo: raise typed exception

            if (compiledExpression == null)
            {
                compiledExpression = compileExpression(new List<Type>{targetType}, (string)parameter, value.GetType());
            
                //try convert back expression
                try
                {
                    var resType = value.GetType() == typeof(Visibility) ? typeof(bool) : value.GetType();
                    var param = System.Linq.Expressions.Expression.Parameter(resType);
                    compiledInversedExpression = new Inverse.Inverter().InverseExpression(compiledExpression.Expression, param);
                }
                catch (Exception e)
                {
                    inverseFaulted = true;
                    throw;
                    //Console.WriteLine("CalcBinding error: {0}", e.Message);
                }
            }
            
            if (targetType == typeof(bool) && value.GetType() == typeof(Visibility))
                value = new BoolToVisibilityConverter(FalseToVisibility)
                    .ConvertBack(value, targetType, null, culture);

            var source = compiledInversedExpression.Invoke(value);

            return source;
        } 

        #endregion
        
        #region IMultiValueConverter
        
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (compiledExpression == null)
            {
                // we can't determine value type if value is null
                // so, binding Path = (a == null) ? "a" : "b" is permitted
                if (values.Any(v => v == null))
                {
                    Console.WriteLine("calcBinding: error: one of source fields is null in binding init, return NULL");
                    return null;
                }
                //todo: questions on this code

                compiledExpression = compileExpression(values.Select(v => v.GetType()).ToList(), (string)parameter, targetType);
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
                    result = String.Format(CultureInfo.InvariantCulture, "{0}", result);
            }

            return result;
        }

        //todo: resultType is not used, delete?
        private Lambda compileExpression(List<Type> argumentsTypes, string expressionTemplate, Type resultType)
        {
            //template = "{1} {2} {0}"
            //argTypes = "BBB AA C"

            for (int i = 0; i < argumentsTypes.Count(); i++)
            {
                expressionTemplate = expressionTemplate.Replace("{" + i.ToString() + "}", getVariableName(i));
            }

            // exprTemplate = "{b c a}"
            //replace: BBB -> a, AA -> b, C -> c
            // input expected: "{a b c}"    | error detected
            // real input: "BBB AA C"          | error detected
            var parametersDefinition = new List<Parameter>();

            for (var i = 0; i < argumentsTypes.Count(); i++)
            {
                parametersDefinition.Add(
                    new Parameter(getVariableName(i), argumentsTypes[i])
                );
            }

            var compiledExpression = parser.Parse(expressionTemplate, parametersDefinition.ToArray());

            return compiledExpression;
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
    }
}
