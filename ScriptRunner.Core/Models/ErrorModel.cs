using System;

namespace ScriptRunner.Core.Models
{
    public class ErrorModel
    {
        public string Message { get; set; }
        public Exception Exception { get; set; }
    }
}
