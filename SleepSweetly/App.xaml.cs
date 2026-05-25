using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Extensions.DependencyInjection;
using SleepSweetly.Infrastructure.Services;
using SleepSweetly.ViewModels;
using SleepSweetly.Views;
using System;
using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Controls;

namespace SleepSweetly
{
    /// <summary>
    /// Главный класс приложения
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider? _serviceProvider;
        private TaskbarIcon? _trayIcon;
        private MainWindow? _mainWindow;

        /// <summary>
        /// Точка входа в приложение
        /// </summary>
        /// <param name="e">Параметры запуска</param>
        [SupportedOSPlatform("windows")]
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            #region настройка DI контейнера
            var services = new ServiceCollection();
            services.AddSingleton<IPowerRequestService, PowerRequestService>();
            services.AddTransient<MainViewModel>();
            services.AddTransient<MainWindow>();
            _serviceProvider = services.BuildServiceProvider();
            #endregion

            _mainWindow = _serviceProvider.GetRequiredService<MainWindow>();

            _trayIcon = new TaskbarIcon
            {
                IconSource = new System.Windows.Media.Imaging.BitmapImage(
                    new Uri("pack://application:,,,/Assets/app.ico")),
                ToolTipText = "SleepSweetly"
            };

            #region контекстное меню
            var menu = new ContextMenu();
            var showItem = new MenuItem { Header = "Показать" };
            var scanItem = new MenuItem { Header = "Сканировать" };
            var exitItem = new MenuItem { Header = "Выход" };

            showItem.Click += (s, args) => ToggleWindow();
            scanItem.Click += (s, args) => _mainWindow?.ViewModel.RefreshCommand.Execute(null);
            exitItem.Click += (s, args) => ExitFromTray();

            menu.Items.Add(showItem);
            menu.Items.Add(scanItem);
            menu.Items.Add(new Separator());
            menu.Items.Add(exitItem);
            #endregion

            _trayIcon.ContextMenu = menu;
            _trayIcon.TrayMouseDoubleClick += (s, args) => ToggleWindow();
        }

        /// <summary>
        /// Показать или скрыть главное окно
        /// </summary>
        public void ToggleWindow()
        {
            if (_mainWindow == null) return;

            if (_mainWindow.IsVisible)
                _mainWindow.Hide();
            else
            {
                _mainWindow.Show();
                _mainWindow.Activate();
            }
        }

        /// <summary>
        /// Выход из приложения
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void ExitFromTray()
        {
            _trayIcon?.Dispose();
            Shutdown();
        }

        /// <summary>
        /// Завершение работы приложения
        /// </summary>
        /// <param name="e">Параметры завершения</param>
        [SupportedOSPlatform("windows")]
        protected override void OnExit(ExitEventArgs e)
        {
            _trayIcon?.Dispose();
            _serviceProvider?.Dispose();
            base.OnExit(e);
        }
    }
}