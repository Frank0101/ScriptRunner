using ScriptRunner.Core.Models;
using ScriptRunner.Core.Repositories;
using ScriptRunner.Core.Services;
using System.Collections.Generic;
using System.Linq;

namespace ScriptRunner.Infra.Services
{
    public class UserScriptsService : IUserScriptsService
    {
        private readonly IScriptsRepository _scriptsRepository;
        private readonly IAuthorizationsRepository _authorizationsRepository;

        public UserScriptsService(IScriptsRepository scriptsRepository,
            IAuthorizationsRepository authorizationsRepository)
        {
            _scriptsRepository = scriptsRepository;
            _authorizationsRepository = authorizationsRepository;
        }

        public IEnumerable<ScriptModel> GetUsersScripts(string userName, IEnumerable<string> userGroups)
        {
            var scripts = _scriptsRepository.GetAll();
            var authorizations = _authorizationsRepository.GetAll();

            return authorizations
                .Where(authorization =>
                    (authorization.Type == AuthorizationType.User && authorization.Target == userName)
                    || (authorization.Type == AuthorizationType.Group && userGroups != null && userGroups.Contains(authorization.Target)))
                .SelectMany(authorization => authorization.Scripts)
                .Distinct().OrderBy(script => script)
                .Select(scriptKey => scripts.FirstOrDefault(script => script.Key == scriptKey));
        }
    }
}
