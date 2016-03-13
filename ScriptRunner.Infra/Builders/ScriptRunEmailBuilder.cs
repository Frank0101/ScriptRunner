using Newtonsoft.Json;
using ScriptRunner.Core.Builders;
using ScriptRunner.Core.Models;
using System.Linq;
using System.Net.Mail;

namespace ScriptRunner.Infra.Builders
{
    public class ScriptRunEmailBuilder : IScriptRunEmailBuilder
    {
        private readonly ConfigModel _config;

        public ScriptRunEmailBuilder(ConfigModel config)
        {
            _config = config;
        }

        public MailMessage Build(ScriptRunResultModel scriptRunResult)
        {
            var emailMessage = new MailMessage
            {
                From = new MailAddress(_config.EmailSender),
                Subject = _config.EmailSubject
                    .Replace("{userName}", scriptRunResult.UserName)
                    .Replace("{scriptKey}", scriptRunResult.ScriptKey)
                    .Replace("{exitCode}", scriptRunResult.ScriptExitCode.ToString()),

                Body = JsonConvert.SerializeObject(scriptRunResult,
                    Formatting.Indented),
            };
            _config.EmailRecipients.ToList().ForEach(emailRecipient
                => emailMessage.To.Add(emailRecipient));

            return emailMessage;
        }
    }
}
