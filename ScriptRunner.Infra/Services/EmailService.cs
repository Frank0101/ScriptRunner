using ScriptRunner.Core.Services;
using System.Net.Mail;

namespace ScriptRunner.Infra.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpClient _smtpClient;

        public EmailService()
        {
            _smtpClient = new SmtpClient();

        }
        public EmailService(string host, int port)
        {
            _smtpClient = new SmtpClient(host, port);
        }

        public void SendEmail(MailMessage emailMessage)
        {
            _smtpClient.Send(emailMessage);
        }
    }
}
