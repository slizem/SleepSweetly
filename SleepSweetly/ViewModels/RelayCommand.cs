using System;
using System.Windows.Input;

namespace SleepSweetly.ViewModels
{
    /// <summary>
    /// Реализация ICommand для MVVM привязок
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Predicate<object?>? _canExecute;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="execute">Метод для выполнения</param>
        /// <param name="canExecute">Метод проверки доступности (опционально)</param>
        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// Проверка доступности команды
        /// </summary>
        /// <param name="parameter">Параметр команды</param>
        public bool CanExecute(object? parameter) => _canExecute == null || _canExecute(parameter);

        /// <summary>
        /// Выполнение команды
        /// </summary>
        /// <param name="parameter">Параметр команды</param>
        public void Execute(object? parameter) => _execute(parameter);

        /// <summary>
        /// Событие изменения доступности команды
        /// </summary>
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        /// <summary>
        /// Принудительный перевызов CanExecute
        /// </summary>
        public void RaiseCanExecuteChanged() => CommandManager.InvalidateRequerySuggested();
    }
}