using Autofac;
using Microsoft.Azure.WebJobs.Host;

namespace Lilybot.Commute.CommuteHomeNotifierJob
{
    public class AutofacActivator : IJobActivator
    {
        private readonly IContainer _container;

        public AutofacActivator(IContainer container)
        {
            _container = container;
        }

        public T CreateInstance<T>()
        {
            return _container.Resolve<T>();
        }
    }
}
