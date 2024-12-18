using Backend.Services;
using DotEnv.Core;
using MailKit.Net.Smtp;
using MimeKit;

namespace Backend.Repositories
{
    public class EmailRepository : IEmailService
    {
        private readonly string? SmtpServer = EnvReader.Instance["SMTP_SERVER"];
        private readonly int SmtpPort = int.Parse(EnvReader.Instance["SMTP_PORT"]);
        private readonly string? SenderEmail = EnvReader.Instance["SMTP_SENDER_EMAIL"];
        private readonly string? SenderPassword = EnvReader.Instance["SMTP_SENDER_PASSWORD"];

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Phuong Uyen Homestay", SenderEmail));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(SmtpServer, SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(SenderEmail, SenderPassword);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}