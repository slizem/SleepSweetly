using SleepSweetly.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace SleepSweetly.Views
{
    public partial class MainWindow : Window
    {
        public MainViewModel ViewModel => (MainViewModel)DataContext;

        public MainWindow(MainViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            Loaded += (s, e) => { PositionWindow(); ViewModel.LoadedCommand.Execute(null); };
            ViewModel.ToggleWindowRequested += () => { if (IsVisible) Hide(); else { Show(); Activate(); ViewModel.RefreshCommand.Execute(null); } };
            ViewModel.ExitRequested += Close;
        }

        /// <summary>
        /// Позиционирование окна
        /// </summary>
        private void PositionWindow()
        {
            var wa = SystemParameters.WorkArea;
            Left = wa.Right - Width - 16;
            Top = wa.Bottom - Height - 16;
        }

        /// <summary>
        /// Перетаскивание окна за левую кнопку мыши
        /// </summary>
        private void OnMouseLeftButtonDown(object s, MouseButtonEventArgs e) { if (e.ButtonState == MouseButtonState.Pressed) DragMove(); }

        protected override void OnClosing(CancelEventArgs e) { e.Cancel = true; Hide(); }
    }
}