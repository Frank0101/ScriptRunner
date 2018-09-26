using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using ScriptRunner.Core.Builders;
using ScriptRunner.Core.Models;
using ScriptRunner.Core.Repositories;
using ScriptRunner.Core.Services;
using ScriptRunner.Infra.Builders;
using ScriptRunner.Infra.Repositories;
using ScriptRunner.Infra.Services;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;

namespace ScriptRunner.Infrastructure
{
    public class CastleInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<ConfigModel>().Instance(new ConfigModel
                {
                    DefaultDelay = Convert.ToInt32(ConfigurationManager.AppSettings["DefaultDelay"]),
                    DefaultTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["DefaultTimeout"]),
                    EmailSender = ConfigurationManager.AppSettings["EmailSender"],
                    EmailSubject = ConfigurationManager.AppSettings["EmailSubject"],
                    EmailRecipients = ConfigurationManager.AppSettings["EmailRecipients"].Split(';')
                        .Select(emailRecipient => emailRecipient.Trim())
                }),

                Component.For<IScriptsRepository>().ImplementedBy<ScriptsRepository>().LifestylePerWebRequest()
                    .DependsOn(new { configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Configuration\ScriptsConfig.xml") }),
                Component.For<IAuthorizationsRepository>().ImplementedBy<AuthorizationsRepository>().LifestylePerWebRequest()
                    .DependsOn(new { configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Configuration\AuthorizationsConfig.xml") }),
                Component.For<IScriptsDynamicDataRepository>().ImplementedBy<ScriptsDynamicDataRepository>().LifestylePerWebRequest()
                    .DependsOn(new { dataFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"DynamicData\ScriptsDynamicData.xml") }),
                Component.For<IUserScriptsService>().ImplementedBy<UserScriptsService>().LifestylePerWebRequest(),
                Component.For<IScriptRunnerService>().ImplementedBy<ScriptRunnerService>().LifestylePerWebRequest(),
                Component.For<IScriptRunEmailBuilder>().ImplementedBy<ScriptRunEmailBuilder>().LifestylePerWebRequest(),
                Component.For<IEmailService>().ImplementedBy<EmailService>().LifestylePerWebRequest());

            container.Register(Classes.FromThisAssembly().BasedOn<Controller>().LifestylePerWebRequest());
            container.Register(Classes.FromThisAssembly().BasedOn<ApiController>().LifestylePerWebRequest());
        }
    }
}
