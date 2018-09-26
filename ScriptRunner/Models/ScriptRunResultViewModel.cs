using System;
using System.Collections.Generic;

namespace ScriptRunner.Models
{
    public class ScriptRunResultViewModel
    {
        public string UserName { get; set; }
        public string ScriptKey { get; set; }
        public DateTime ScriptStartedAt { get; set; }
        public DateTime ScriptEndedAt { get; set; }
        public int ScriptExitCode { get; set; }
        public string ScriptTermination { get; set; }
        public IEnumerable<string> ScriptOutput { get; set; }
    }
}
