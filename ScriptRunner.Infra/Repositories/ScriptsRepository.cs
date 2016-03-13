using ScriptRunner.Core.Models;
using ScriptRunner.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ScriptRunner.Infra.Repositories
{
    public class ScriptsRepository : IScriptsRepository
    {
        private readonly ConfigModel _config;
        private readonly string _configFilePath;

        public ScriptsRepository(ConfigModel config, string configFilePath)
        {
            _config = config;
            _configFilePath = configFilePath;
        }

        public IEnumerable<ScriptModel> GetAll()
        {
            var xmlConfig = XDocument.Load(_configFilePath);
            return xmlConfig.Root.Elements("script").Select(script => new ScriptModel
            {
                Key = script.Attribute("key").Value,
                Name = script.Attribute("name") != null
                    ? script.Attribute("name").Value
                    : string.Empty,
                Description = script.Attribute("description") != null
                    ? script.Attribute("description").Value
                    : string.Empty,
                Path = script.Attribute("path").Value,
                Arguments = script.Attribute("arguments") != null
                    ? script.Attribute("arguments").Value
                    : string.Empty,
                Delay = script.Attribute("delay") != null
                    ? Convert.ToInt32(script.Attribute("delay").Value)
                    : _config.DefaultDelay,
                Timeout = script.Attribute("timeout") != null
                    ? Convert.ToInt32(script.Attribute("timeout").Value)
                    : _config.DefaultTimeout
            });
        }
    }
}
