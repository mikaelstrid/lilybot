using System.Data.Entity;
using Autofac;
using Lilybot.Core.Application;
using Lilybot.Core.Infrastructure.Persistence.EntityFramework;
using Lilybot.Shopping.Application;
using Lilybot.Shopping.Domain;

namespace Lilybot.Shopping.Infrastructure
{
    public class Bootstrapper
    {
        public static void Bootstrap(ContainerBuilder builder)
        {
            builder.RegisterType<ShoppingDbContext>().As<DbContext>().InstancePerLifetimeScope();

            builder.RegisterType<EntityFrameworkShoppingProfileRepository>().As<IShoppingProfileRepository>();
            builder.RegisterType<EntityFrameworkStoreRepository>().As<IStoreRepository>();
            builder.RegisterType<EntityFrameworkAggregateRepository<Product>>().As<IAggregateRepository<Product>>();
            builder.RegisterType<EntityFrameworkEventRepository>().As<IEventRepository>();
        }
    }
}
