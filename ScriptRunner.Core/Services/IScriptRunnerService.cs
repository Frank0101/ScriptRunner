using ScriptRunner.Core.Models;

namespace ScriptRunner.Core.Services
{
    public interface IScriptRunnerService
    {
        ScriptRunResultModel RunScript(string userName, ScriptModel targetScript);
    }
}
