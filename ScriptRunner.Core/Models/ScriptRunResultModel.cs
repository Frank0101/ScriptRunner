using System;
using System.Collections.Generic;

namespace ScriptRunner.Core.Models
{
    public class ScriptRunResultModel
    {
        public string UserName { get; set; }
        public string ScriptKey { get; set; }
        public DateTime ScriptStartedAt { get; set; }
        public DateTime ScriptEndedAt { get; set; }
        public int ScriptExitCode { get; set; }
        public ScriptTermination ScriptTermination { get; set; }
        public IEnumerable<ScriptOutputModel> ScriptOutput { get; set; }
    }

    public enum ScriptTermination
    {
        ExecutionFinished,
        TimeoutExpired
    }
}
