using System.Threading.Tasks;
using SleepSweetly.Models;

namespace SleepSweetly.Infrastructure.Services
{
    /// <summary>
    /// Сервис получения блокировок сна
    /// </summary>
    public interface IPowerRequestService
    {
        /// <summary>
        /// Получить список блокирующих процессов
        /// </summary>
        Task<ScanResult> GetBlockingRequestsAsync();
    }
}