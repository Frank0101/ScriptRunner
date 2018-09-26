using NUnit.Framework;
using ScriptRunner.Core.Models;
using ScriptRunner.Core.Repositories;
using ScriptRunner.Infra.Repositories;
using System;
using System.IO;
using System.Linq;

namespace ScriptRunner.Test.Unit
{
    public class ScriptsRepositoryTest
    {
        [TestFixture]
        public class When_A_ScriptsRepository_Is_Available
        {
            private readonly string _configFilePath =
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestData\ScriptsConfig.xml");
            private ConfigModel _config;
            private IScriptsRepository _scriptsRepository;

            [SetUp]
            public void SetUp()
            {
                _config = new ConfigModel
                {
                    DefaultDelay = 5,
                    DefaultTimeout = 60
                };
                _scriptsRepository = new ScriptsRepository(_config, _configFilePath);
            }

            [Test]
            public void It_Should_Get_All_The_Scripts()
            {
                var scripts = _scriptsRepository.GetAll();

                Assert.AreEqual(3, scripts.Count());
                Assert.AreEqual("test-script-1", scripts.First().Key);
                Assert.AreEqual("Test Script 1", scripts.First().Name);
                Assert.AreEqual("Description Test Script 1", scripts.First().Description);
                Assert.AreEqual(@"C:\test-script-1.bat", scripts.First().Path);
                Assert.AreEqual("arguments-1", scripts.First().Arguments);
                Assert.AreEqual(1, scripts.First().Delay);
                Assert.AreEqual(10, scripts.First().Timeout);
            }

            [Test]
            public void It_Should_Return_The_Default_Delay_If_Delay_Is_Not_Defined()
            {
                var scripts = _scriptsRepository.GetAll();

                Assert.AreEqual(5, scripts.ElementAt(1).Delay);
                Assert.AreEqual(60, scripts.ElementAt(1).Timeout);
            }
        }
    }
}
