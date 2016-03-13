using ScriptRunner.Core.Models;
using ScriptRunner.Core.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ScriptRunner.Infra.Repositories
{
    public class AuthorizationsRepository : IAuthorizationsRepository
    {
        private readonly string _configFilePath;

        public AuthorizationsRepository(string configFilePath)
        {
            _configFilePath = configFilePath;
        }

        public IEnumerable<AuthorizationModel> GetAll()
        {
            var xmlConfig = XDocument.Load(_configFilePath);
            return xmlConfig.Root.Elements("authorization").Select(authorization => new AuthorizationModel
            {
                Target = authorization.Attribute("target").Value,
                Type = authorization.Attribute("type").Value == "user"
                    ? AuthorizationType.User
                    : AuthorizationType.Group,
                Scripts = authorization.Elements("script").Select(script => script.Attribute("key").Value)
            });
        }
    }
}
