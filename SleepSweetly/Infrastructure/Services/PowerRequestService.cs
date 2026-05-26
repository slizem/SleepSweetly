using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using SleepSweetly.Models;

namespace SleepSweetly.Infrastructure.Services
{
    public class PowerRequestService : IPowerRequestService
    {
        private const string PowerCfgCommand = "powercfg.exe";
        private const string PowerCfgArguments = "-requests";

        /// <summary>
        /// Получает список процессов, блокирующих сон
        /// </summary>
        public async Task<ScanResult> GetBlockingRequestsAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = GetBlockingApps();

                    if (result == "CLEAR")
                    {
                        return new ScanResult
                        {
                            HasBlockers = false,
                            BlockingRequests = new System.Collections.Generic.List<PowerRequest>(),
                            ScanTime = DateTime.Now
                        };
                    }

                    var requests = ParseOldFormat(result);

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
        /// Запуск команды
        /// </summary>
        private string GetBlockingApps()
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = PowerCfgCommand,
                    Arguments = PowerCfgArguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8
                };

                using (var process = Process.Start(psi))
                {
                    var output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    return ParsePowerRequests(output);
                }
            }
            catch (Exception ex)
            {
                return $"ERROR:{ex.Message}";
            }
        }

        /// <summary>
        /// Парсит вывод powercfg в упрощенный формат
        /// </summary>
        /// <param name="output">Сырой вывод powercfg -requests</param>
        /// <returns>Строка с блокировками в формате [категория]\nпроцесс\n\n или "CLEAR"</returns>
        private string ParsePowerRequests(string output)
        {
            var lines = output.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            var result = new StringBuilder();
            string currentCategory = null;
            bool hasBlockers = false;

            foreach (var raw in lines)
            {
                var line = raw.Trim();

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                if (line.EndsWith(":"))
                {
                    var cat = line.TrimEnd(':').Trim();
                    if (cat == "DISPLAY" || cat == "SYSTEM" || cat == "AWAYMODE" ||
                        cat == "ВЫПОЛНЕНИЕ" || cat == "EXECUTION")
                    {
                        currentCategory = cat;
                    }
                    continue;
                }

                if (line.Contains("[PROCESS]") || line.Contains("[ПРОЦЕСС]"))
                {
                    var process = line.Replace("[PROCESS]", "").Replace("[ПРОЦЕСС]", "").Trim();
                    if (!string.IsNullOrWhiteSpace(process))
                    {
                        hasBlockers = true;
                        result.AppendLine($"[{currentCategory}]");
                        result.AppendLine(process);
                        result.AppendLine();
                    }
                    continue;
                }

                if (currentCategory != null && (line.Contains(".exe") || line.Contains("\\Device")))
                {
                    hasBlockers = true;
                    result.AppendLine($"[{currentCategory}]");
                    result.AppendLine(line);
                    result.AppendLine();
                }
            }

            return hasBlockers ? result.ToString() : "CLEAR";
        }

        /// <summary>
        /// Преобразует упрощенный формат в список PowerRequest
        /// </summary>
        /// <param name="data">Строка с блокировками в формате [категория]\nпроцесс\n\n</param>
        private System.Collections.Generic.List<PowerRequest> ParseOldFormat(string data)
        {
            var requests = new System.Collections.Generic.List<PowerRequest>();
            var lines = data.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            string currentCategory = null;

            foreach (var line in lines)
            {
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    currentCategory = line.Trim('[', ']');
                }
                else if (!string.IsNullOrWhiteSpace(line) && currentCategory != null)
                {
                    requests.Add(new PowerRequest
                    {
                        Category = currentCategory switch
                        {
                            "DISPLAY" or "ДИСПЛЕЙ" => PowerCategory.Display,
                            "SYSTEM" or "СИСТЕМА" => PowerCategory.System,
                            "EXECUTION" or "ВЫПОЛНЕНИЕ" => PowerCategory.Execution,
                            "AWAYMODE" => PowerCategory.AwayMode,
                            _ => PowerCategory.Unknown
                        },
                        ProcessName = line.Trim(),
                        RawDescription = line.Trim()
                    });
                }
            }

            return requests;
        }
    }
}