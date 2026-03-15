using System;

namespace YourProjectName.Models
{
    /// <summary>
    /// Telegram bot configuration model with obfuscation
    /// </summary>
    public class TelegramConfiguration
    {
        private string _r5k;
        private string _c7j;

        public string BotToken
        {
            get => _u2m(_r5k);
            set => _r5k = value;
        }

        public string ChatId
        {
            get => _u2m(_c7j);
            set => _c7j = value;
        }

        public string EncryptionKey { get; set; }
        public string ApiEndpoint { get; set; } = "https://api.telegram.org";
        public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromSeconds(30);

        // Obfuscated getter
        private string _u2m(string val) => string.IsNullOrWhiteSpace(val) ? string.Empty : val;
    }
}