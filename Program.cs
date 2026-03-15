using System;
using System.Windows.Forms;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using YourProjectName.Services;

namespace YourProjectName
{
    static class Program
    {
        private static IServiceProvider _serviceProvider;

        [STAThread]
        static void Main()
        {
            // Configure Dependency Injection
            _serviceProvider = ConfigureServices();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var form = new Form1();
                    Application.Run(form);
                }
            }
            catch (Exception ex)
            {
                var logger = _serviceProvider.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "Application terminated unexpectedly");
                MessageBox.Show("Application Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Load configuration from App.config and appsettings.json
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            services.AddSingleton<IConfiguration>(configuration);

            // Add Logging
            services.AddLogging(config =>
            {
                config.AddConsole();
                config.AddDebug();
                config.SetMinimumLevel(LogLevel.Information);
            });

            // Add HttpClient
            services.AddHttpClient();
            services.AddScoped<HttpClient>();

            // Register Telegram Bot Service as Singleton
            services.AddSingleton<ITelegramBotService, TelegramBotService>();

            // Register Background Worker (optional)
            services.AddSingleton<IHostedService, TelegramNotificationWorker>();

            return services.BuildServiceProvider();
        }

        public static IServiceProvider GetServiceProvider() => _serviceProvider;
    }
}