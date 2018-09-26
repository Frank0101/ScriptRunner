namespace ScriptRunner.Core.Models
{
    public class ScriptOutputErrModel : ScriptOutputModel
    {
        public override string Type
        {
            get { return "StdErr"; }
        }
    }
}
