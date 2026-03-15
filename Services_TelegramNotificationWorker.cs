using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using YourProjectName.Services;

namespace YourProjectName.Services
{
    /// <summary>
    /// Background worker for sending periodic Telegram notifications
    /// </summary>
    public class TelegramNotificationWorker : BackgroundService
    {
        private readonly ITelegramBotService _svc;
        private readonly ILogger<TelegramNotificationWorker> _log;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(5);

        public TelegramNotificationWorker(
            ITelegramBotService telegramService,
            ILogger<TelegramNotificationWorker> logger)
        {
            _svc = telegramService;
            _log = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _log?.LogInformation("Telegram Notification Worker started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Send periodic notification
                    var msg = $"System Status Report\nTime: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\nStatus: Running";
                    await _svc.SendMessageAsync(msg);

                    // Wait for the specified interval
                    await Task.Delay(_interval, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _log?.LogError(ex, "Error in Telegram notification worker");
                    // Wait before retry
                    await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                }
            }

            _log?.LogInformation("Telegram Notification Worker stopped");
        }
    }
}