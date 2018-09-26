using Castle.Windsor;
using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace ScriptRunner.Infrastructure
{
    public class CastleControllerFactory : DefaultControllerFactory
    {
        public IWindsorContainer Container { get; private set; }

        public CastleControllerFactory(IWindsorContainer container)
        {
            if (container != null)
            {
                Container = container;
            }
            else
            {
                throw new ArgumentNullException("container");
            }
        }

        protected override IController
            GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            //Resolves Controller
            if (controllerType != null)
            {
                return Container.Resolve(controllerType) as IController;
            }
            return null;
        }

        public override void ReleaseController(IController controller)
        {
            if (controller is IDisposable)
            {
                ((IDisposable)controller).Dispose();
            }

            //Releases Controller
            Container.Release(controller);
        }
    }
}
