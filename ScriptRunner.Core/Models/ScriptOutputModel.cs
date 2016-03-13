using System;

namespace ScriptRunner.Core.Models
{
    public abstract class ScriptOutputModel
    {
        public abstract string Type { get; }
        public DateTime Timestamp { get; set; }
        public string Value { get; set; }
    }
}
