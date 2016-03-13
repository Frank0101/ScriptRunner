using System;

namespace ScriptRunner.Core.Exceptions
{
    public class DelayNotExpiredException : Exception
    {
        public double RemainingDelay { get; set; }
    }
}
