using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using SleepSweetly.Infrastructure.Services;
using SleepSweetly.Models;

namespace SleepSweetly.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IPowerRequestService _service;
        private bool _isScanning;
        private string _statusText = "Готов";
        private string _lastUpdateText = "";
        private bool _hasError;
        private ObservableCollection<PowerRequest> _requests = new();

        public MainViewModel(IPowerRequestService service)
        {
            _service = service;
            RefreshCommand = new RelayCommand(_ => ExecuteRefresh(), _ => !IsScanning);
            ToggleWindowCommand = new RelayCommand(_ => ToggleWindowRequested?.Invoke());
            ExitCommand = new RelayCommand(_ => ExitRequested?.Invoke());
            LoadedCommand = new RelayCommand(_ => ExecuteRefresh());
        }

        public ObservableCollection<PowerRequest> BlockingRequests { get => _requests; set => SetProperty(ref _requests, value); }
        public bool IsScanning { 
            get => _isScanning; private set {
                if (SetProperty(ref _isScanning, value))
                    (RefreshCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public string StatusText { get => _statusText; set => SetProperty(ref _statusText, value); }
        public string LastUpdateText { get => _lastUpdateText; set => SetProperty(ref _lastUpdateText, value); }
        public bool HasError { get => _hasError; set => SetProperty(ref _hasError, value); }

        public ICommand RefreshCommand { get; }
        public ICommand ToggleWindowCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand LoadedCommand { get; }

        public event Action ToggleWindowRequested;
        public event Action ExitRequested;

        /// <summary>
        /// Запуск сканирования блокировок
        /// </summary>
        private async void ExecuteRefresh()
        {
            if (IsScanning) return;
            IsScanning = true;
            StatusText = "Сканирование...";
            HasError = false;
            BlockingRequests.Clear();

            try
            {
                var result = await _service.GetBlockingRequestsAsync();
                if (result.IsError)
                {
                    HasError = true;
                    StatusText = "Ошибка";
                    LastUpdateText = $"Ошибка: {result.ErrorMessage}";
                }
                else
                {
                    foreach (var r in result.BlockingRequests) BlockingRequests.Add(r);
                    StatusText = result.HasBlockers ? "Обнаружены блокировки" : "Сон не блокируется";
                    LastUpdateText = $"Последняя проверка: {result.ScanTime:HH:mm:ss}";
                }
            }
            catch (Exception ex)
            {
                HasError = true;
                StatusText = "Ошибка";
                LastUpdateText = $"Ошибка: {ex.Message}";
            }
            finally { IsScanning = false; }
        }
    }
}