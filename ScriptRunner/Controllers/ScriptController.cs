using ScriptRunner.Core.Builders;
using ScriptRunner.Core.Exceptions;
using ScriptRunner.Core.Models;
using ScriptRunner.Core.Services;
using ScriptRunner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web.Http;

namespace ScriptRunner.Controllers
{
    [Authorize]
    [RoutePrefix("script")]
    public class ScriptController : ApiController
    {
        private readonly IUserScriptsService _userScriptsService;
        private readonly IScriptRunnerService _scriptRunnerService;
        private readonly IScriptRunEmailBuilder _scriptRunEmailBuilder;
        private readonly IEmailService _emailService;

        public ScriptController(IUserScriptsService userScriptsService,
            IScriptRunnerService scriptRunnerService,
            IScriptRunEmailBuilder scriptRunEmailBuilder,
            IEmailService emailService)
        {
            _userScriptsService = userScriptsService;
            _scriptRunnerService = scriptRunnerService;
            _scriptRunEmailBuilder = scriptRunEmailBuilder;
            _emailService = emailService;
        }

        [HttpGet]
        [Route("list")]
        public object List()
        {
            try
            {
                var user = User.Identity as WindowsIdentity;
                var userScripts = GetUserScripts(user);

                return userScripts.Select(userScript => new ScriptViewModel
                {
                    Key = userScript.Key,
                    Name = userScript.Name,
                    Description = userScript.Description,
                    RunLink = string.Format("{0}/script/run/{1}",
                        Request.RequestUri.GetLeftPart(UriPartial.Authority),
                        userScript.Key)
                });
            }
            catch (Exception e)
            {
                return new ErrorModel
                {
                    Message = e.Message,
                    Exception = e
                };
            }
        }

        [HttpGet]
        [Route("run/{key}")]
        public object Run(string key)
        {
            try
            {
                var user = User.Identity as WindowsIdentity;
                var userScript = GetUserScript(user, key);

                if (userScript != null)
                {
                    var scriptRunResult = _scriptRunnerService.RunScript(user.Name, userScript);
                    _emailService.SendEmail(_scriptRunEmailBuilder.Build(scriptRunResult));

                    return new ScriptRunResultViewModel
                    {
                        UserName = scriptRunResult.UserName,
                        ScriptKey = scriptRunResult.ScriptKey,
                        ScriptStartedAt = scriptRunResult.ScriptStartedAt,
                        ScriptEndedAt = scriptRunResult.ScriptEndedAt,
                        ScriptTermination = scriptRunResult.ScriptTermination == ScriptTermination.ExecutionFinished
                            ? "Execution finished"
                            : "Timeout expired",
                        ScriptOutput = scriptRunResult.ScriptOutput.Select(output => string.Format("{0} - {1}: {2}",
                            output.Timestamp, output.Type, output.Value))
                    };
                }
                return new ErrorModel
                {
                    Message = "The script was not found or the user is not authorized",
                };
            }
            catch (DelayNotExpiredException e)
            {
                return new ErrorModel
                {
                    Message = string.Format(
                        "The script has been run recently. Please wait {0} seconds",
                        e.RemainingDelay),
                };
            }
            catch (Exception e)
            {
                return new ErrorModel
                {
                    Message = e.Message,
                    Exception = e
                };
            }
        }

        private IEnumerable<ScriptModel> GetUserScripts(WindowsIdentity user)
        {
            return _userScriptsService.GetUsersScripts(user.Name,
                user.Groups.Select(group => @group.Value));
        }

        private ScriptModel GetUserScript(WindowsIdentity user, string key)
        {
            return GetUserScripts(user)
                .FirstOrDefault(script => script.Key == key);
        }
    }
}
