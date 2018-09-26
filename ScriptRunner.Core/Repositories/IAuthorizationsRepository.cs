using ScriptRunner.Core.Models;
using System.Collections.Generic;

namespace ScriptRunner.Core.Repositories
{
    public interface IAuthorizationsRepository
    {
        IEnumerable<AuthorizationModel> GetAll();
    }
}
