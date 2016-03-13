using System.Collections.Generic;

namespace ScriptRunner.Core.Models
{
    public class ConfigModel
    {
        public int DefaultDelay { get; set; }
        public int DefaultTimeout { get; set; }

        public string EmailSender { get; set; }
        public string EmailSubject { get; set; }
        public IEnumerable<string> EmailRecipients { get; set; }
    }
}
