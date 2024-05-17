using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;
using System.Net.Mail;

namespace AddressIPControlBackgroundService.Helpers
{
    public class EmailSender
    {
        private readonly ILogger<Worker> _logger;

        public EmailSender(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        public async Task SendEmailAsync(string smtpServer, int port, string fromEmail, string password, string toEmail, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Sender Name", fromEmail));
            message.To.Add(new MailboxAddress("Recipient Name", toEmail));
            message.Subject = subject;

            message.Body = new TextPart("html")
            {
                Text = body
            };

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(smtpServer, port, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(fromEmail, password);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                    _logger.LogInformation($"Wysłano email: {body}, na adres: {toEmail}");
                }
                catch (Exception ex)
                {
                    _logger.LogError("Podczas wysyłania email występił błąd! " + ex.Message);
                }
            }
        }
    }
}
