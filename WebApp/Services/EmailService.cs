using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using WebApp.Settings;

namespace WebApp.Services
{
    public class EmailService(IOptions<SmtpSetting> smtpSetting) : IEmailService
    {
        public async Task SendAsync(string email, string subject, string body, string v)
        {
            var message = new MailMessage(
                "rabbifazla4@gmail.com",
                "",
                subject,
                body
                );
            message.IsBodyHtml = true;

            using (var emailClient = new SmtpClient(smtpSetting.Value.Host, smtpSetting.Value.Port))
            {
                emailClient.Credentials = new NetworkCredential(
                    smtpSetting.Value.User,
                    smtpSetting.Value.Password
                );

                await emailClient.SendMailAsync(message);
            }

        }
    }
}
