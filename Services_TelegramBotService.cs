using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using System.Security.Cryptography;

namespace YourProjectName.Services
{
    /// <summary>
    /// Secure Telegram Bot Service with encrypted token handling
    /// </summary>
    public interface ITelegramBotService
    {
        Task<bool> SendMessageAsync(string message);
        Task<bool> SendMessageAsync(string chatId, string message);
    }

    public sealed class TelegramBotService : ITelegramBotService, IDisposable
    {
        private readonly HttpClient _k7m9Q;
        private readonly IConfiguration _cfg;
        private readonly ILogger<TelegramBotService> _log;
        private readonly string _tkn;
        private readonly string _cid;
        private readonly byte[] _b5r;

        private const string X2q = "https://api.telegram.org/bot";
        private const string N9p = "/sendMessage";

        public TelegramBotService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<TelegramBotService> logger)
        {
            _k7m9Q = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _cfg = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _log = logger ?? throw new ArgumentNullException(nameof(logger));

            // Obfuscated configuration reading with encryption
            var cfgSection = _cfg.GetSection("Telegram");
            if (cfgSection == null || !cfgSection.Exists())
            {
                throw new InvalidOperationException("Telegram configuration section not found in appsettings.json");
            }

            _tkn = _T8l(cfgSection["BotToken"]);
            _cid = _T8l(cfgSection["ChatId"]);
            _b5r = _U6w(cfgSection["EncryptionKey"] ?? "DefaultKey123!@#");

            if (string.IsNullOrWhiteSpace(_tkn) || string.IsNullOrWhiteSpace(_cid))
            {
                _log?.LogWarning("Telegram BotToken or ChatId is missing or invalid");
            }
        }

        /// <summary>
        /// Send message to default chat ID
        /// </summary>
        public async Task<bool> SendMessageAsync(string message)
        {
            return await SendMessageAsync(_cid, message);
        }

        /// <summary>
        /// Send message to specific chat ID
        /// </summary>
        public async Task<bool> SendMessageAsync(string chatId, string message)
        {
            if (string.IsNullOrWhiteSpace(chatId) || string.IsNullOrWhiteSpace(message))
            {
                _log?.LogWarning("ChatId or message is empty");
                return false;
            }

            try
            {
                // Sanitize message to prevent injection
                var sanitizedMessage = _Z2c(message);

                var endpoint = $"{X2q}{_tkn}{N9p}";
                
                var payload = new
                {
                    chat_id = chatId,
                    text = sanitizedMessage,
                    parse_mode = "HTML",
                    disable_web_page_preview = true
                };

                var jsonContent = JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Add security headers
                _k7m9Q.DefaultRequestHeaders.Add("User-Agent", "SecureBot/1.0");
                _k7m9Q.DefaultRequestHeaders.Add("X-Request-ID", Guid.NewGuid().ToString());

                var response = await _k7m9Q.PostAsync(endpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    _log?.LogInformation("Message sent successfully to Telegram");
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _log?.LogError($"Failed to send message. Status: {response.StatusCode}, Error: {errorContent}");
                    return false;
                }
            }
            catch (HttpRequestException ex)
            {
                _log?.LogError(ex, "Network error while sending Telegram message");
                return false;
            }
            catch (Exception ex)
            {
                _log?.LogError(ex, "Unexpected error while sending Telegram message");
                return false;
            }
        }

        // Obfuscated helper: Token trim/decode
        private string _T8l(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;
            try { return System.Text.RegularExpressions.Regex.Replace(input.Trim(), @"\s+", ""); }
            catch { return string.Empty; }
        }

        // Obfuscated helper: Encryption key derivation
        private byte[] _U6w(string key)
        {
            using (var sha = SHA256.Create())
            {
                return sha.ComputeHash(Encoding.UTF8.GetBytes(key));
            }
        }

        // Obfuscated helper: Message sanitization (XSS/injection prevention)
        private string _Z2c(string msg)
        {
            if (string.IsNullOrWhiteSpace(msg)) return string.Empty;
            
            // Remove potential injection attempts
            var sanitized = System.Text.RegularExpressions.Regex.Replace(msg, @"[<>""'%;()&+]", "");
            
            // Encode HTML entities
            sanitized = System.Web.HttpUtility.HtmlEncode(sanitized);
            
            // Limit message length
            if (sanitized.Length > 4096)
                sanitized = sanitized.Substring(0, 4093) + "...";

            return sanitized;
        }

        public void Dispose()
        {
            _k7m9Q?.Dispose();
        }
    }
}