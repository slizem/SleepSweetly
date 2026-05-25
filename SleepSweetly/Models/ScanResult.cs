using System;
using System.Collections.Generic;

namespace SleepSweetly.Models
{
    /// <summary>
    /// Результат сканирования блокировок сна
    /// </summary>
    public class ScanResult
    {
        public bool HasBlockers { get; set; }
        public List<PowerRequest> BlockingRequests { get; set; } = new();
        public DateTime ScanTime { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsError => !string.IsNullOrEmpty(ErrorMessage);
    }
}