using Castle.Windsor;
using ScriptRunner.Infrastructure;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ScriptRunner
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //Creates Castle Container
            var container = new WindsorContainer();
            container.Install(new CastleInstaller());

            //Creates Castle Controller Factory (MVC)
            var castleControllerFactory = new CastleControllerFactory(container);
            ControllerBuilder.Current.SetControllerFactory(castleControllerFactory);

            //Creates Castle Controller Factory (Web API)
            var castleDependenctResolver = new CastleDependencyResolver(container.Kernel);
            GlobalConfiguration.Configuration.DependencyResolver = castleDependenctResolver;
        }
    }
}
