using NUnit.Framework;
using ScriptRunner.Core.Services;
using ScriptRunner.Infra.Services;
using System.Net.Mail;

namespace ScriptRunner.Test.Unit
{
    public class EmailServiceTest
    {
        [TestFixture]
        public class When_An_EmailService_Is_Available
        {
            private IEmailService _emailService;

            [SetUp]
            public void SetUp()
            {
                _emailService = new EmailService("localhost", 25);
            }

            [Test]
            public void It_Should_Send_An_Email_Message()
            {
                Assert.DoesNotThrow(() =>
                {
                    var emailMessage = new MailMessage
                    {
                        From = new MailAddress("script-runner@test.com"),
                        Subject = "subject - EmailServiceTest",
                        Body = "body - EmailServiceTest",
                    };
                    emailMessage.To.Add(new MailAddress("recipient@test.com"));
                    _emailService.SendEmail(emailMessage);
                });
            }
        }
    }
}
