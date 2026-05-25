using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SleepSweetly.Infrastructure.Converters
{
    /// <summary>
    /// Конвертер bool -> Visibility
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Конвертирует bool в Visibility
        /// </summary>
        /// <param name="value">bool значение</param>
        /// <param name="targetType">Тип назначения</param>
        /// <param name="parameter">Доп. параметр</param>
        /// <param name="culture">Культура</param>
        /// <returns>Visible если true, иначе Collapsed</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            (value is bool b && b) ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>
        /// Конвертирует Visibility в bool
        /// </summary>
        /// <param name="value">Visibility значение</param>
        /// <param name="targetType">Тип назначения</param>
        /// <param name="parameter">Доп. параметр</param>
        /// <param name="culture">Культура</param>
        /// <returns>true если Visible, иначе false</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            (value is Visibility v && v == Visibility.Visible);
    }
}