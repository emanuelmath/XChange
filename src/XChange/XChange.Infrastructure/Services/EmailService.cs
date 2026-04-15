using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using XChange.Core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace XChange.Infrastructure.Services
{
    public class EmailService(IConfiguration config) : IEmailService
    {
        public async Task<bool> SendEmailAsync(string toName, string toEmail, string subject, string body, string htmlBody)
        {
            var senderEmail = config["EmailConfig:SenderEmail"];
            var appPassword = config["EmailConfig:AppPassword"];
            var smtpServer = config["EmailConfig:SmtpServer"];
            var port = int.Parse(config["EmailConfig:Port"]!);

            var msg = new MimeMessage();
            msg.From.Add(new MailboxAddress("XChange", senderEmail));
            msg.To.Add(new MailboxAddress(toName, toEmail));
            msg.Subject = subject; 

            var builder = new BodyBuilder();

            builder.TextBody = body;

            builder.HtmlBody = htmlBody;

            msg.Body = builder.ToMessageBody();

            try
            {
                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(smtpServer, port, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(senderEmail, appPassword);
                await smtp.SendAsync(msg);
                await smtp.DisconnectAsync(true);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
