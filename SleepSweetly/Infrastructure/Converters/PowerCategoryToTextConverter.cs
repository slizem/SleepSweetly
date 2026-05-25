using System;
using System.Globalization;
using System.Windows.Data;
using SleepSweetly.Models;

namespace SleepSweetly.Infrastructure.Converters
{
    /// <summary>
    /// Конвертер категории в текст
    /// </summary>
    [ValueConversion(typeof(PowerCategory), typeof(string))]
    public class PowerCategoryToTextConverter : IValueConverter
    {
        private const string DisplayText = "Дисплей";
        private const string SystemText = "Система";
        private const string ExecutionText = "Выполнение";
        private const string AwayModeText = "Away Mode";
        private const string PerfBoostText = "Производительность";
        private const string ActiveLockScreenText = "Экран блокировки";
        private const string UnknownText = "";

        /// <summary>
        /// Конвертирует PowerCategory в строку
        /// </summary>
        /// <param name="value">PowerCategory</param>
        /// <param name="targetType">Тип назначения</param>
        /// <param name="parameter">Доп. параметр</param>
        /// <param name="culture">Культура</param>
        /// <returns>Строковое представление категории</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is PowerCategory cat)
                return cat switch
                {
                    PowerCategory.Display => DisplayText,
                    PowerCategory.System => SystemText,
                    PowerCategory.Execution => ExecutionText,
                    PowerCategory.AwayMode => AwayModeText,
                    PowerCategory.PerfBoost => PerfBoostText,
                    PowerCategory.ActiveLockScreen => ActiveLockScreenText,
                    _ => UnknownText
                };
            return UnknownText;
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