using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;

namespace CalcBinding
{
    /// <summary>
    /// Common BoolToVisibility converter with FalseToVisibility parameter
    /// </summary>
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
            var valueType = value.GetType();

            if (valueType != typeof(bool))
            {
                var implicitConverter = GetImplicitConversion(valueType, typeof(bool));
                if (implicitConverter != null)
                {
                    value = implicitConverter.Invoke(null, new object[] { value });
                }
            }

            if ((bool)value)
                return Visibility.Visible;

            return (FalseToVisibility == FalseToVisibility.Collapsed) ? Visibility.Collapsed : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Visibility)value == Visibility.Visible;
        }

        public FalseToVisibility FalseToVisibility { get; set; }

        private static MethodInfo GetImplicitConversion(Type baseType, Type targetType)
        {
            return GetAllImplicitCasts(baseType, targetType)
                .FirstOrDefault();
        }

        private static IEnumerable<MethodInfo> GetAllImplicitCasts(Type baseType, Type targetType)
        {
            var currentTypeMethods = baseType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(x => x.Name == "op_Implicit" && x.ReturnType == targetType)
                .ToList();

            while (baseType != typeof(object) && !currentTypeMethods.Any())
            {
                return GetAllImplicitCasts(baseType.BaseType, targetType);
            }

            return currentTypeMethods;
        }
    }
}
