using NUnit.Framework;
using ScriptRunner.Core.Models;
using ScriptRunner.Core.Repositories;
using ScriptRunner.Infra.Repositories;
using System;
using System.IO;
using System.Linq;

namespace ScriptRunner.Test.Unit
{
    public class ScriptsDynamicDataRepositoryTest
    {
        [TestFixture]
        public class When_A_ScriptsDynamicDataRepository_Is_Available
        {
            private readonly string _dataFilePath =
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestData\ScriptsDynamicData.xml");
            private IScriptsDynamicDataRepository _scriptsDynamicDataRepository;

            [SetUp]
            public void SetUp()
            {
                _scriptsDynamicDataRepository = new ScriptsDynamicDataRepository(_dataFilePath);

                if (File.Exists(_dataFilePath))
                {
                    File.Delete(_dataFilePath);
                }
            }

            [Test]
            public void It_Should_Write_And_Read_A_Script_Dynamic_Data()
            {
                var scriptDynamicData = new ScriptDynamicDataModel
                {
                    Key = "test-script-1",
                    LastRun = DateTime.UtcNow.Ticks
                };

                var result = _scriptsDynamicDataRepository.GetAll();
                Assert.IsNotNull(result);
                Assert.AreEqual(0, result.Count());

                //Create
                _scriptsDynamicDataRepository.Set(scriptDynamicData);

                result = _scriptsDynamicDataRepository.GetAll();
                Assert.IsNotNull(result);
                Assert.AreEqual(1, result.Count());
                Assert.AreEqual(scriptDynamicData.Key, result.First().Key);
                Assert.AreEqual(scriptDynamicData.LastRun, result.First().LastRun);

                //Update
                scriptDynamicData.LastRun += 100;
                _scriptsDynamicDataRepository.Set(scriptDynamicData);

                result = _scriptsDynamicDataRepository.GetAll();
                Assert.IsNotNull(result);
                Assert.AreEqual(1, result.Count());
                Assert.AreEqual(scriptDynamicData.Key, result.First().Key);
                Assert.AreEqual(scriptDynamicData.LastRun, result.First().LastRun);
            }

            [TearDown]
            public void TearDown()
            {
                if (File.Exists(_dataFilePath))
                {
                    File.Delete(_dataFilePath);
                }
            }
        }
    }
}
