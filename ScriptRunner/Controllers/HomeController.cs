using ScriptRunner.Models;
using System;
using System.Reflection;
using System.Security.Principal;
using System.Web.Http;

namespace ScriptRunner.Controllers
{
    [Authorize]
    [RoutePrefix("")]
    public class HomeController : ApiController
    {
        [HttpGet]
        [Route("")]
        public HomeViewModel Index()
        {
            var user = User.Identity as WindowsIdentity;
            return new HomeViewModel
            {
                ApiName = "Script Runner",
                UserName = user.Name,
                Version = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                ScriptsLink = string.Format("{0}/script/list", Request.RequestUri.GetLeftPart(UriPartial.Authority)),
            };
        }
    }
}
