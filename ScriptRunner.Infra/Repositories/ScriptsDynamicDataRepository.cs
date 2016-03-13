using ScriptRunner.Core.Models;
using ScriptRunner.Core.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace ScriptRunner.Infra.Repositories
{
    public class ScriptsDynamicDataRepository : IScriptsDynamicDataRepository
    {
        private readonly string _dataFilePath;

        public ScriptsDynamicDataRepository(string dataFilePath)
        {
            _dataFilePath = dataFilePath;
        }

        public IEnumerable<ScriptDynamicDataModel> GetAll()
        {
            if (File.Exists(_dataFilePath))
            {
                var xmlConfig = XDocument.Load(_dataFilePath);
                return xmlConfig.Root.Elements("script-dynamic-data").Select(script => new ScriptDynamicDataModel
                {
                    Key = script.Attribute("key").Value,
                    LastRun = Convert.ToInt64(script.Attribute("last-run").Value)
                }).ToList();
            }
            return new List<ScriptDynamicDataModel>();
        }

        public void Set(ScriptDynamicDataModel scriptDynamicData)
        {
            var scriptsDynamicData = (IList<ScriptDynamicDataModel>)GetAll();
            if (scriptsDynamicData.Any(script => script.Key == scriptDynamicData.Key))
            {
                scriptsDynamicData.Remove(scriptsDynamicData.FirstOrDefault(script => script.Key == scriptDynamicData.Key));
            }
            scriptsDynamicData.Add(scriptDynamicData);

            var xmlData =
                new XDocument(
                    new XElement("scripts-dynamic-data",
                        scriptsDynamicData.Select(script => new XElement("script-dynamic-data",
                            new XAttribute("key", script.Key), new XAttribute("last-run", script.LastRun)
                        )
                    )
                )
            );

            xmlData.Save(_dataFilePath);
        }
    }
}
