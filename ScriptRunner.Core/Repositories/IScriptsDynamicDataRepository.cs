using ScriptRunner.Core.Models;
using System.Collections.Generic;

namespace ScriptRunner.Core.Repositories
{
    public interface IScriptsDynamicDataRepository
    {
        IEnumerable<ScriptDynamicDataModel> GetAll();
        void Set(ScriptDynamicDataModel scriptDynamicData);
    }
}
