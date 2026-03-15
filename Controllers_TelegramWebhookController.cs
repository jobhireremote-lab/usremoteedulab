using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using YourProjectName.Services;

namespace YourProjectName.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TelegramWebhookController : ControllerBase
    {
        private readonly ITelegramBotService _v4x;
        private readonly ILogger<TelegramWebhookController> _q8r;

        public TelegramWebhookController(
            ITelegramBotService telegramService,
            ILogger<TelegramWebhookController> logger)
        {
            _v4x = telegramService;
            _q8r = logger;
        }

        /// <summary>
        /// Handle incoming Telegram webhook updates
        /// </summary>
        [HttpPost("update")]
        public async Task<IActionResult> _M3n()
        {
            try
            {
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();

                if (string.IsNullOrWhiteSpace(body))
                {
                    _q8r?.LogWarning("Empty webhook body received");
                    return Ok();
                }

                // Parse the Telegram update
                using var doc = JsonDocument.Parse(body);
                var root = doc.RootElement;

                if (root.TryGetProperty("message", out var message))
                {
                    var msgText = message.GetProperty("text").GetString() ?? "No text";
                    var chatId = message.GetProperty("chat").GetProperty("id").GetInt64().ToString();
                    var userId = message.GetProperty("from").GetProperty("id").GetInt64();

                    _q8r?.LogInformation($"Received message from user {userId}: {msgText}");

                    // Process the message
                    var response = await _P9w(msgText, chatId);

                    // Send response back
                    await _v4x.SendMessageAsync(chatId, response);
                }

                return Ok();
            }
            catch (JsonException jex)
            {
                _q8r?.LogError(jex, "Invalid JSON in webhook body");
                return BadRequest("Invalid JSON");
            }
            catch (Exception ex)
            {
                _q8r?.LogError(ex, "Error processing Telegram webhook");
                return StatusCode(500, "Internal Server Error");
            }
        }

        /// <summary>
        /// Health check endpoint for webhook
        /// </summary>
        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
        }

        // Obfuscated helper: Message processing logic
        private async Task<string> _P9w(string userMsg, string chatId)
        {
            // Simulate command processing
            var cmd = userMsg?.ToLower().Trim() ?? string.Empty;

            var response = cmd switch
            {
                "/start" => "Welcome to the bot! Type /help for available commands.",
                "/help" => "Available commands:\n/start - Start the bot\n/status - Check system status\n/help - Show this message",
                "/status" => $"System Status: Online ✅\nTimestamp: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}",
                _ => "I don't understand that command. Type /help for available commands."
            };

            return await Task.FromResult(response);
        }
    }
}