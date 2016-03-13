using ScriptRunner.Core.Models;
using System.Collections.Generic;

namespace ScriptRunner.Core.Services
{
    public interface IUserScriptsService
    {
        IEnumerable<ScriptModel> GetUsersScripts(string userName, IEnumerable<string> userGroups);
    }
}
