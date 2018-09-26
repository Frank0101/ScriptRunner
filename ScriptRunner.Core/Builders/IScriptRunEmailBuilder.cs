using ScriptRunner.Core.Models;
using System.Net.Mail;

namespace ScriptRunner.Core.Builders
{
    public interface IScriptRunEmailBuilder
    {
        MailMessage Build(ScriptRunResultModel scriptRunResult);
    }
}
