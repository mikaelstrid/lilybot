using System;
using System.IO;
using Autofac;
using Lilybot.Shopping.Infrastructure;
using Microsoft.Azure.WebJobs;

namespace Lilybot.Shopping.UpdateItemCountJob
{
    public class Functions
    {
        [NoAutomaticTrigger]
        public static void UpdateItemCounts(TextWriter log)
        {
            try
            {
                log.WriteLine($"{DateTime.UtcNow.ToString("HH:mm:ss")} UpdateItemCounts is started.");

                var builder = new ContainerBuilder();
                Bootstrapper.Bootstrap(builder);
                builder.RegisterInstance(log);
                builder.RegisterType<ItemManager>().As<IItemManager>();
                var container = builder.Build();

                using (container.BeginLifetimeScope())
                {
                    var itemManager = container.Resolve<IItemManager>();
                    itemManager.UpdateItemCounts();
                }

                log.WriteLine($"{DateTime.UtcNow.ToString("HH:mm:ss")}: UpdateItemCounts is done.");

            }
            catch (Exception e)
            {
                log.WriteLine($"Error: {e}");
            }
        }
    }
}
