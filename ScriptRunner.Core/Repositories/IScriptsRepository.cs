using ScriptRunner.Core.Models;
using System.Collections.Generic;

namespace ScriptRunner.Core.Repositories
{
    public interface IScriptsRepository
    {
        IEnumerable<ScriptModel> GetAll();
    }
}
