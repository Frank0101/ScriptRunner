using NUnit.Framework;
using ScriptRunner.Core.Models;
using ScriptRunner.Core.Repositories;
using ScriptRunner.Infra.Repositories;
using System;
using System.IO;
using System.Linq;

namespace ScriptRunner.Test.Unit
{
    public class AuthorizationsRepositoryTest
    {
        [TestFixture]
        public class When_An_AuthorizationsRepository_Is_Available
        {
            private readonly string _configFilePath =
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestData\AuthorizationsConfig.xml");
            private IAuthorizationsRepository _authorizationsRepository;

            [SetUp]
            public void SetUp()
            {
                _authorizationsRepository = new AuthorizationsRepository(_configFilePath);
            }

            [Test]
            public void It_Should_Get_All_The_Authorizations()
            {
                var authorizations = _authorizationsRepository.GetAll();

                Assert.AreEqual(2, authorizations.Count());
                Assert.AreEqual("user-1", authorizations.First().Target);
                Assert.AreEqual(AuthorizationType.User, authorizations.First().Type);
                Assert.AreEqual(2, authorizations.First().Scripts.Count());
                Assert.AreEqual("test-script-2", authorizations.First().Scripts.First());
            }
        }
    }
}
