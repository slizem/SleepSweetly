using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SleepSweetly.Models;

namespace SleepSweetly.Infrastructure.Services
{
    public class PowerRequestService : IPowerRequestService
    {
        private const string PowerCfgCommand = "powercfg.exe";
        private const string PowerCfgArguments = "-requests";

        /// <summary>
        /// Получает список блокирующих сон процессов
        /// </summary>
        public async Task<ScanResult> GetBlockingRequestsAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    var output = ExecutePowerCfg();
                    var requests = ParsePowerCfgOutput(output);
                    return new ScanResult
                    {
                        HasBlockers = requests.Count > 0,
                        BlockingRequests = requests,
                        ScanTime = DateTime.Now
                    };
                }
                catch (Exception ex)
                {
                    return new ScanResult
                    {
                        HasBlockers = false,
                        ScanTime = DateTime.Now,
                        ErrorMessage = ex.Message
                    };
                }
            });
        }

        /// <summary>
        /// Выполняет команду powercfg -requests
        /// </summary>
        private string ExecutePowerCfg()
        {
            var psi = new ProcessStartInfo
            {
                FileName = PowerCfgCommand,
                Arguments = PowerCfgArguments,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8
            };

            using (var process = Process.Start(psi))
            {
                if (process == null)
                    return string.Empty;

                var output = process.StandardOutput.ReadToEnd();
                process.WaitForExit(5000);
                return output ?? string.Empty;
            }
        }

        /// <summary>
        /// Парсит вывод powercfg в список блокировок
        /// </summary>
        /// <param name="output">Вывод команды powercfg -requests</param>
        private List<PowerRequest> ParsePowerCfgOutput(string output)
        {
            var requests = new List<PowerRequest>();
            if (string.IsNullOrEmpty(output))
                return requests;

            var lines = output.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            PowerCategory currentCategory = PowerCategory.Unknown;

            foreach (var line in lines)
            {
                if (TryParseCategory(line, out var category))
                {
                    currentCategory = category;

                    continue;
                }

                if (TryParseProcessRequest(line, out var procName, out var procPath))
                {
                    requests.Add(new PowerRequest
                    {
                        Category = currentCategory,
                        ProcessName = procName ?? "Unknown",
                        ExecutablePath = procPath,
                        RawDescription = line.Trim()
                    });
                }

                else if (TryParseGenericRequest(line, out var desc))
                {
                    requests.Add(new PowerRequest
                    {
                        Category = currentCategory,
                        RawDescription = desc,
                        ProcessName = ExtractProcessName(desc) ?? "Unknown"
                    });
                }
            }

            return requests;
        }

        /// <summary>
        /// Парсит категорию блокировки из строки
        /// </summary>
        /// <param name="line">Строка с категорией</param>
        /// <param name="category">Распарсенная категория</param>
        private bool TryParseCategory(string line, out PowerCategory category)
        {
            category = PowerCategory.Unknown;

            if (string.IsNullOrEmpty(line) || !line.EndsWith(":"))
                return false;

            var text = line.TrimEnd(':').Trim().ToUpperInvariant();
            category = text switch
            {
                "DISPLAY" or "ДИСПЛЕЙ" => PowerCategory.Display,
                "SYSTEM" or "СИСТЕМА" => PowerCategory.System,
                "EXECUTION" or "ВЫПОЛНЕНИЕ" => PowerCategory.Execution,
                "AWAYMODE" => PowerCategory.AwayMode,
                "PERFBOOST" => PowerCategory.PerfBoost,
                "ACTIVELOCKSCREEN" => PowerCategory.ActiveLockScreen,
                _ => PowerCategory.Unknown
            };

            return category != PowerCategory.Unknown;
        }

        /// <summary>
        /// Парсит строку с процессом в формате [PROCESS] имя
        /// </summary>
        /// <param name="line">Строка с процессом</param>
        /// <param name="procName">Имя процесса</param>
        /// <param name="procPath">Путь к процессу</param>
        private bool TryParseProcessRequest(string line, out string? procName, out string? procPath)
        {
            procName = null;
            procPath = null;

            if (string.IsNullOrEmpty(line))
                return false;

            var match = Regex.Match(line, @"\[PROCESS\]\s*(.+?)(?:\.exe)?");

            if (!match.Success)
                return false;

            procName = match.Groups[1].Value.Trim();
            procPath = procName;

            return true;
        }

        /// <summary>
        /// Парсит обычную строку блокировки (не процесс)
        /// </summary>
        /// <param name="line">Строка блокировки</param>
        /// <param name="desc">Описание блокировки</param>
        private bool TryParseGenericRequest(string line, out string? desc)
        {
            desc = null;

            if (string.IsNullOrEmpty(line))
                return false;

            var skip = new[] { "Нет.", "Ни один.", "None.", "сек)", "min)" };

            foreach (var s in skip)
                if (line.Contains(s))
                    return false;

            desc = line.Trim();

            return true;
        }

        /// <summary>
        /// Извлекает имя .exe из строки описания
        /// </summary>
        /// <param name="desc">Строка описания</param>
        private string? ExtractProcessName(string desc)
        {
            if (string.IsNullOrEmpty(desc))
                return null;

            var match = Regex.Match(desc, @"([a-zA-Z0-9_\-]+\.exe)", RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[1].Value : null;
        }
    }
}