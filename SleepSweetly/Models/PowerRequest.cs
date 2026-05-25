namespace SleepSweetly.Models
{
    /// <summary>
    /// Информация о процессе, блокирующем сон
    /// </summary>
    public class PowerRequest
    {
        public PowerCategory Category { get; set; }
        public string ProcessName { get; set; }
        public string ExecutablePath { get; set; }
        public string RawDescription { get; set; }


        /// <summary>
        /// Отображаемое имя процесса
        /// </summary>
        public string DisplayName => string.IsNullOrEmpty(ProcessName) ? RawDescription ?? "Unknown" : ProcessName;
    }
}