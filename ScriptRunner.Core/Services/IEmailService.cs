using System.Net.Mail;

namespace ScriptRunner.Core.Services
{
    public interface IEmailService
    {
        void SendEmail(MailMessage emailMessage);
    }
}
