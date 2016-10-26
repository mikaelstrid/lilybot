using System.Data.Entity;
using Autofac;
using Lilybot.Commute.Application;
using Lilybot.Commute.Domain;
using Lilybot.Core.Application;
using Lilybot.Core.Infrastructure.Persistence.EntityFramework;

namespace Lilybot.Commute.Infrastructure
{
    public class Bootstrapper
    {
        public static void Bootstrap(ContainerBuilder builder)
        {
            builder.RegisterType<CommuteDbContext>().As<DbContext>().InstancePerLifetimeScope();
            builder.RegisterType<EntityFrameworkAggregateRepository<CommuteProfile>>().As<IAggregateRepository<CommuteProfile>>();
            builder.RegisterType<SlackMessageSender>().As<ISlackMessageSender>();
            //builder.RegisterType<FamilyRepository>().As<IFamilyProfileRepository>();
        }
    }
}
