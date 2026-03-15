using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using YourProjectName.Services;

namespace YourProjectName
{
    public partial class Form1 : Form
    {
        private readonly ITelegramBotService _telegramService;

        public Form1()
        {
            InitializeComponent();

            // Get service from DI container
            var serviceProvider = Program.GetServiceProvider();
            _telegramService = serviceProvider.GetRequiredService<ITelegramBotService>();
        }

        // Example: Send message on button click
        private async void btnSendTelegramMessage_Click(object sender, EventArgs e)
        {
            try
            {
                string message = txtMessage.Text;
                bool success = await _telegramService.SendMessageAsync(message);

                if (success)
                {
                    MessageBox.Show("Message sent to Telegram successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtMessage.Clear();
                }
                else
                {
                    MessageBox.Show("Failed to send message to Telegram.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}