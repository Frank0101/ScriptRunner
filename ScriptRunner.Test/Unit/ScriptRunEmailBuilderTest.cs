using Newtonsoft.Json;
using NUnit.Framework;
using ScriptRunner.Core.Builders;
using ScriptRunner.Core.Models;
using ScriptRunner.Infra.Builders;
using System;
using System.Collections.Generic;

namespace ScriptRunner.Test.Unit
{
    public class ScriptRunEmailBuilderTest
    {
        [TestFixture]
        public class When_An_ScriptRunEmailBuilder_Is_Available
        {
            private ConfigModel _config;
            private IScriptRunEmailBuilder _scriptRunEmailBuilder;

            [SetUp]
            public void SetUp()
            {
                _config = new ConfigModel
                {
                    EmailSender = "script-runner@test.com",
                    EmailSubject = "subject - {userName} - {scriptKey}",
                    EmailRecipients = new List<string>()
                    {
                        "test-recipient-1@test.com",
                        "test-recipient-2@test.com",
                        "test-recipient-3@test.com"
                    }
                };
                _scriptRunEmailBuilder = new ScriptRunEmailBuilder(_config);
            }

            [Test]
            public void It_Should_Generate_An_Email_Message()
            {
                var scriptRunResult = new ScriptRunResultModel
                {
                    UserName = "user-1",
                    ScriptKey = "test-script-1",
                    ScriptStartedAt = new DateTime(2015, 12, 11, 12, 0, 0),
                    ScriptEndedAt = new DateTime(2015, 12, 11, 12, 0, 3),
                    ScriptOutput = new List<ScriptOutputModel>
                    {
                        new ScriptOutputStdModel
                        {
                            Timestamp = new DateTime(2015, 12, 11, 12, 0, 1),
                            Value = "output 1"
                        },
                        new ScriptOutputStdModel
                        {
                            Timestamp = new DateTime(2015, 12, 11, 12, 0, 2),
                            Value = "output 2"
                        }
                    }
                };
                var emailMessage = _scriptRunEmailBuilder.Build(scriptRunResult);

                Assert.AreEqual("script-runner@test.com", emailMessage.From.Address);
                Assert.AreEqual("subject - user-1 - test-script-1", emailMessage.Subject);
                Assert.AreEqual(JsonConvert.SerializeObject(scriptRunResult,
                    Formatting.Indented), emailMessage.Body);
                Assert.AreEqual(3, emailMessage.To.Count);
                Assert.AreEqual("test-recipient-1@test.com", emailMessage.To[0].Address);
                Assert.AreEqual("test-recipient-2@test.com", emailMessage.To[1].Address);
                Assert.AreEqual("test-recipient-3@test.com", emailMessage.To[2].Address);
            }
        }
    }
}
