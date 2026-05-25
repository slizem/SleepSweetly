using System;
using System.Globalization;
using System.Windows.Data;

namespace SleepSweetly.Infrastructure.Converters
{
    /// <summary>
    /// Инвертирует bool значение
    /// </summary>
    [ValueConversion(typeof(bool), typeof(bool))]
    public class InverseBoolConverter : IValueConverter
    {
        /// <summary>
        /// Инвертирует bool значение
        /// </summary>
        /// <param name="value">bool для инверсии</param>
        /// <param name="targetType">Тип назначения</param>
        /// <param name="parameter">Доп. параметр</param>
        /// <param name="culture">Культура</param>
        /// <returns>Инвертированное bool значение</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            (value is bool b) ? !b : true;

        /// <summary>
        /// Обратная инверсия bool значения
        /// </summary>
        /// <param name="value">bool для инверсии</param>
        /// <param name="targetType">Тип назначения</param>
        /// <param name="parameter">Доп. параметр</param>
        /// <param name="culture">Культура</param>
        /// <returns>Инвертированное bool значение</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            (value is bool b) ? !b : false;
    }
}