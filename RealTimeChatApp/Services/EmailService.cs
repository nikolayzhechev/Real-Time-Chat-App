using RealTimeChatApp.Interfaces;
using SendGrid;

namespace RealTimeChatApp.Services
{
    public class EmailService : IEmailService
    {
        private readonly SendGridClient _sendGridClient;
        private readonly string _fromEmail;

        public EmailService(IConfiguration configuration)
        {
            var apiKey = configuration["SendGrid:ApiKey"];
            _fromEmail = configuration["SendGrid:FromEmail"];
            _sendGridClient = new SendGridClient(apiKey);
        }

        public async Task SendResetPasswordEmail(string to, string resetLink)
        {
            var from = new SendGrid.Helpers.Mail.EmailAddress(_fromEmail, "Real-Time Chat App");
            var subject = "Password Reset Request";
            var toEmail = new SendGrid.Helpers.Mail.EmailAddress(to);
            var plainTextContent = $"Click the following link to reset your password: {resetLink}";
            var htmlContent = $"<strong>Click the following link to reset your password:</strong> <a href='{resetLink}'>Reset Password</a>";
            var msg = SendGrid.Helpers.Mail.MailHelper.CreateSingleEmail(from, toEmail, subject, plainTextContent, htmlContent);
            var response = await _sendGridClient.SendEmailAsync(msg);
        }
    }
}
