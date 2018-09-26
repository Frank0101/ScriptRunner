using Castle.MicroKernel;
using Castle.MicroKernel.Lifestyle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Dependencies;

namespace ScriptRunner.Infrastructure
{
    public class CastleDependencyScope : IDependencyScope
    {
        private readonly IKernel _kernel;
        private readonly IDisposable _disposable;

        public CastleDependencyScope(IKernel kernel)
        {
            _kernel = kernel;
            _disposable = kernel.BeginScope();
        }

        public object GetService(Type type)
        {
            return _kernel.HasComponent(type) ? _kernel.Resolve(type) : null;
        }

        public IEnumerable<object> GetServices(Type type)
        {
            return _kernel.ResolveAll(type).Cast<object>();
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}
