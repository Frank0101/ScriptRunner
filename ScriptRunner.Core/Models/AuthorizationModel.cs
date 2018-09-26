using System.Collections.Generic;

namespace ScriptRunner.Core.Models
{
    public class AuthorizationModel
    {
        public string Target { get; set; }
        public AuthorizationType Type { get; set; }
        public IEnumerable<string> Scripts { get; set; }
    }

    public enum AuthorizationType
    {
        User,
        Group
    }
}
