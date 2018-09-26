namespace ScriptRunner.Core.Models
{
    public class ScriptModel
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }
        public string Arguments { get; set; }
        public int Delay { get; set; }
        public int Timeout { get; set; }
    }
}
