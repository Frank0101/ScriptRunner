using Moq;
using NUnit.Framework;
using ScriptRunner.Core.Models;
using ScriptRunner.Core.Repositories;
using ScriptRunner.Core.Services;
using ScriptRunner.Infra.Services;
using System.Collections.Generic;
using System.Linq;

namespace ScriptRunner.Test.Unit
{
    public class UserScriptsServiceTest
    {
        [TestFixture]
        public class When_An_UserScriptsService_Is_Available
        {
            private Mock<IScriptsRepository> _scriptsRepositoryMock;
            private Mock<IAuthorizationsRepository> _authorizationsRepositoryMock;
            private IUserScriptsService _userScriptsService;

            [SetUp]
            public void SetUp()
            {
                _scriptsRepositoryMock = new Mock<IScriptsRepository>();
                _scriptsRepositoryMock.Setup(o => o.GetAll()).Returns(new List<ScriptModel>
                {
                    new ScriptModel {Key = "test-script-1"},
                    new ScriptModel {Key = "test-script-2"},
                    new ScriptModel {Key = "test-script-3"},
                    new ScriptModel {Key = "test-script-4"},
                    new ScriptModel {Key = "test-script-5"},
                });

                _authorizationsRepositoryMock = new Mock<IAuthorizationsRepository>();
                _authorizationsRepositoryMock.Setup(o => o.GetAll()).Returns(new List<AuthorizationModel>
                {
                    new AuthorizationModel{ Target = "user-1", Type = AuthorizationType.User, Scripts = new List<string>
                    {
                        "test-script-1"
                    }},
                    new AuthorizationModel{ Target = "group-1", Type = AuthorizationType.Group, Scripts = new List<string>
                    {
                        "test-script-2", "test-script-3"
                    }},
                    new AuthorizationModel{ Target = "group-2", Type = AuthorizationType.Group, Scripts = new List<string>
                    {
                        "test-script-3", "test-script-4", "test-script-5"
                    }},
                });

                _userScriptsService = new UserScriptsService(_scriptsRepositoryMock.Object,
                    _authorizationsRepositoryMock.Object);
            }

            [Test]
            public void It_Should_Get_The_User_Script_Based_On_UserName()
            {
                var userScripts = _userScriptsService.GetUsersScripts("user-1", null);

                Assert.AreEqual(1, userScripts.Count());
                Assert.IsTrue(userScripts.Any(userScript => userScript.Key == "test-script-1"));
            }

            [Test]
            public void It_Should_Get_The_User_Script_Based_On_UserGroup()
            {
                var userScripts = _userScriptsService.GetUsersScripts("user-2", new List<string>
                {
                    "group-1"
                });

                Assert.AreEqual(2, userScripts.Count());
                Assert.IsTrue(userScripts.Any(userScript => userScript.Key == "test-script-2"));
                Assert.IsTrue(userScripts.Any(userScript => userScript.Key == "test-script-3"));
            }

            [Test]
            public void It_Should_Get_The_User_Script_Based_On_UserGroups()
            {
                var userScripts = _userScriptsService.GetUsersScripts("user-2", new List<string>
                {
                    "group-1", "group-2"
                });

                Assert.AreEqual(4, userScripts.Count());
                Assert.IsTrue(userScripts.Any(userScript => userScript.Key == "test-script-2"));
                Assert.IsTrue(userScripts.Any(userScript => userScript.Key == "test-script-3"));
                Assert.IsTrue(userScripts.Any(userScript => userScript.Key == "test-script-4"));
                Assert.IsTrue(userScripts.Any(userScript => userScript.Key == "test-script-5"));
            }

            [Test]
            public void It_Should_Get_The_User_Script_Based_On_UserName_And_UserGroups()
            {
                var userScripts = _userScriptsService.GetUsersScripts("user-1", new List<string>
                {
                    "group-1", "group-2"
                });

                Assert.AreEqual(5, userScripts.Count());
                Assert.IsTrue(userScripts.Any(userScript => userScript.Key == "test-script-1"));
                Assert.IsTrue(userScripts.Any(userScript => userScript.Key == "test-script-2"));
                Assert.IsTrue(userScripts.Any(userScript => userScript.Key == "test-script-3"));
                Assert.IsTrue(userScripts.Any(userScript => userScript.Key == "test-script-4"));
                Assert.IsTrue(userScripts.Any(userScript => userScript.Key == "test-script-5"));
            }
        }
    }
}
