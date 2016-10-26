using System.Configuration;
using Autofac;
using Lilybot.Commute.Infrastructure;
using Microsoft.Azure.WebJobs;
using Microsoft.ServiceBus;

namespace Lilybot.Commute.CommuteHomeNotifierJob
{
    public class Program
    {
        //private static string _servicesBusConnectionString;

        //private static NamespaceManager _namespaceManager;

        public static void Main()
        {
            //_servicesBusConnectionString = ConfigurationManager.ConnectionStrings["AzureWebJobsServiceBus"].ConnectionString;
            //_namespaceManager = NamespaceManager.CreateFromConnectionString(_servicesBusConnectionString);

            var builder = new ContainerBuilder();
            Bootstrapper.Bootstrap(builder);
            LocalBootstrapper.Bootstrap(builder);
            var container = builder.Build();

            var config = new JobHostConfiguration()
            {
                //ServiceBusConnectionString = _servicesBusConnectionString,
                JobActivator = new AutofacActivator(container)
            };
            config.UseServiceBus();

            var host = new JobHost(config);
            host.RunAndBlock();
        }
    }
}
