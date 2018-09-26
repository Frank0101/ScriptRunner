using Moq;
using NUnit.Framework;
using ScriptRunner.Core.Exceptions;
using ScriptRunner.Core.Models;
using ScriptRunner.Core.Repositories;
using ScriptRunner.Core.Services;
using ScriptRunner.Infra.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ScriptRunner.Test.Unit
{
    public class ScriptRunnerServiceTest
    {
        [TestFixture]
        public class When_A_ScriptRunnerService_Is_Available
        {
            private Mock<IScriptsDynamicDataRepository> _scriptsDynamicDataRepositoryMock;
            private IScriptRunnerService _scriptRunnerService;

            [SetUp]
            public void SetUp()
            {
                _scriptsDynamicDataRepositoryMock = new Mock<IScriptsDynamicDataRepository>();
                _scriptsDynamicDataRepositoryMock.Setup(o => o.GetAll()).Returns(new List<ScriptDynamicDataModel>
                {
                    new ScriptDynamicDataModel
                    {
                        Key = "test-script-2",
                        LastRun = DateTime.UtcNow.AddSeconds(-10).Ticks
                    }
                });

                _scriptRunnerService = new ScriptRunnerService(_scriptsDynamicDataRepositoryMock.Object);
            }

            [Test]
            public void It_Should_Run_A_Script_And_Return_The_Output()
            {
                var scriptRunResult = _scriptRunnerService.RunScript("user-1", new ScriptModel
                {
                    Key = "test-script-1",
                    Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestScripts\test-script-with-echo.bat"),
                    Timeout = 60
                });

                Assert.AreEqual("user-1", scriptRunResult.UserName);
                Assert.AreEqual("test-script-1", scriptRunResult.ScriptKey);
                Assert.AreEqual(0, scriptRunResult.ScriptExitCode);
                Assert.AreEqual(ScriptTermination.ExecutionFinished, scriptRunResult.ScriptTermination);
                Assert.AreEqual(9, scriptRunResult.ScriptOutput.Count(output => output is ScriptOutputStdModel));
                Assert.AreEqual(0, scriptRunResult.ScriptOutput.Count(output => output is ScriptOutputErrModel));
                Assert.IsTrue(scriptRunResult.ScriptOutput.Any(output => output.Value == "test output 1"));
                Assert.IsTrue(scriptRunResult.ScriptOutput.Any(output => output.Value == "test output 2"));
                Assert.IsTrue(scriptRunResult.ScriptOutput.Any(output => output.Value == "test output 3"));
            }

            [Test]
            public void It_Should_Run_A_Script_With_Arguments_And_Return_The_Output()
            {
                var scriptRunResult = _scriptRunnerService.RunScript("user-1", new ScriptModel
                {
                    Key = "test-script-1",
                    Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestScripts\test-script-with-args.bat"),
                    Arguments = "test-argument-1 test-argument-2 test-argument-3",
                    Timeout = 60
                });

                Assert.AreEqual("user-1", scriptRunResult.UserName);
                Assert.AreEqual("test-script-1", scriptRunResult.ScriptKey);
                Assert.AreEqual(0, scriptRunResult.ScriptExitCode);
                Assert.AreEqual(ScriptTermination.ExecutionFinished, scriptRunResult.ScriptTermination);
                Assert.AreEqual(9, scriptRunResult.ScriptOutput.Count(output => output is ScriptOutputStdModel));
                Assert.AreEqual(0, scriptRunResult.ScriptOutput.Count(output => output is ScriptOutputErrModel));
                Assert.IsTrue(scriptRunResult.ScriptOutput.Any(output => output.Value == "test-argument-1"));
                Assert.IsTrue(scriptRunResult.ScriptOutput.Any(output => output.Value == "test-argument-2"));
                Assert.IsTrue(scriptRunResult.ScriptOutput.Any(output => output.Value == "test-argument-3"));
            }

            [Test]
            public void It_Should_Run_A_Script_With_Errors_And_Return_The_Output()
            {
                var scriptRunResult = _scriptRunnerService.RunScript("user-1", new ScriptModel
                {
                    Key = "test-script-1",
                    Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestScripts\test-script-with-errors.bat"),
                    Timeout = 60
                });

                Assert.AreEqual("user-1", scriptRunResult.UserName);
                Assert.AreEqual("test-script-1", scriptRunResult.ScriptKey);
                Assert.AreEqual(0, scriptRunResult.ScriptExitCode);
                Assert.AreEqual(ScriptTermination.ExecutionFinished, scriptRunResult.ScriptTermination);
                Assert.AreEqual(13, scriptRunResult.ScriptOutput.Count(output => output is ScriptOutputStdModel));
                Assert.AreEqual(4, scriptRunResult.ScriptOutput.Count(output => output is ScriptOutputErrModel));
                Assert.IsTrue(scriptRunResult.ScriptOutput.Any(output => output.Value == "test output 1"));
                Assert.IsTrue(scriptRunResult.ScriptOutput.Any(output => output.Value == "test output 2"));
                Assert.IsTrue(scriptRunResult.ScriptOutput.Any(output => output.Value == "test output 3"));
            }

            [Test]
            public void It_Should_Run_A_Script_With_Delays_And_Return_The_Output()
            {
                var scriptRunResult = _scriptRunnerService.RunScript("user-1", new ScriptModel
                {
                    Key = "test-script-1",
                    Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestScripts\test-script-with-delays.bat"),
                    Timeout = 60
                });

                Assert.AreEqual("user-1", scriptRunResult.UserName);
                Assert.AreEqual("test-script-1", scriptRunResult.ScriptKey);
                Assert.AreEqual(0, scriptRunResult.ScriptExitCode);
                Assert.AreEqual(ScriptTermination.ExecutionFinished, scriptRunResult.ScriptTermination);
                Assert.AreEqual(13, scriptRunResult.ScriptOutput.Count(output => output is ScriptOutputStdModel));
                Assert.AreEqual(0, scriptRunResult.ScriptOutput.Count(output => output is ScriptOutputErrModel));
                Assert.IsTrue(scriptRunResult.ScriptOutput.Any(output => output.Value == "test output 1"));
                Assert.IsTrue(scriptRunResult.ScriptOutput.Any(output => output.Value == "test output 2"));
                Assert.IsTrue(scriptRunResult.ScriptOutput.Any(output => output.Value == "test output 3"));
            }

            [Test]
            public void It_Should_Run_The_Script_If_Delay_Has_Expired()
            {
                var e = Assert.Throws<DelayNotExpiredException>(() => _scriptRunnerService.RunScript("user-1", new ScriptModel
                {
                    Key = "test-script-2",
                    Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestScripts\test-script-with-echo.bat"),
                    Delay = 15,
                    Timeout = 60
                }));
                Assert.IsTrue(e.RemainingDelay > 4.5 && e.RemainingDelay < 5.5);
            }

            [Test]
            public void It_Should_Throw_An_Exception_If_Delay_Has_Not_Expired()
            {
                Assert.DoesNotThrow(() => _scriptRunnerService.RunScript("user-1", new ScriptModel
                {
                    Key = "test-script-2",
                    Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestScripts\test-script-with-echo.bat"),
                    Delay = 5,
                    Timeout = 60
                }));
            }

            [Test]
            public void It_Should_Interrupt_The_Execution_After_The_Timeout()
            {
                var scriptRunResult = _scriptRunnerService.RunScript("user-1", new ScriptModel
                {
                    Key = "test-script-1",
                    Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestScripts\test-script-with-delays.bat"),
                    Timeout = 1
                });

                Assert.AreEqual(ScriptTermination.TimeoutExpired, scriptRunResult.ScriptTermination);
            }
        }
    }
}
