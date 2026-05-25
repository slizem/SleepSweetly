using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SleepSweetly.Infrastructure.Converters
{
    /// <summary>
    /// Показывает элемент если коллекция пуста
    /// </summary>
    [ValueConversion(typeof(int), typeof(Visibility))]
    public class CollectionEmptyToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Конвертирует количество элементов в Visibility
        /// </summary>
        /// <param name="value">количество элементов или коллекция</param>
        /// <param name="targetType">Тип назначения</param>
        /// <param name="parameter">Доп. параметр</param>
        /// <param name="culture">Культура</param>
        /// <returns>Visible если коллекция пуста, иначе Collapsed</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int count = value is int i ? i : (value is ICollection c ? c.Count : 0);
            return count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Обратная конвертация (не реализована)
        /// </summary>
        /// <param name="value">Значение</param>
        /// <param name="targetType">Тип назначения</param>
        /// <param name="parameter">Доп. параметр</param>
        /// <param name="culture">Культура</param>
        /// <returns>Всегда исключение</returns>
        /// <exception cref="NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}