using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace CalcBinding
{
    /// <summary>
    /// Bool to visibility converter (common) with FalseToVisibility parameter
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
}
