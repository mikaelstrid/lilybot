using System.Data.Entity;
using Autofac;
using Lily.Core.Application;
using Lily.Core.Infrastructure.Persistence.EntityFramework;
using Lily.ShoppingList.Application;
using Lily.ShoppingList.Domain;

namespace Lily.ShoppingList.Infrastructure
{
    public class Bootstrapper
    {
        public static void Bootstrap(ContainerBuilder builder)
        {
            builder.RegisterType<ShoppingListDbContext>().As<DbContext>().InstancePerLifetimeScope();

            builder.RegisterType<EntityFrameworkProfileRepository>().As<IProfileRepository>();
            builder.RegisterType<EntityFrameworkStoreRepository>().As<IStoreRepository>();
            builder.RegisterType<EntityFrameworkAggregateRepository<Product>>().As<IAggregateRepository<Product>>();
            builder.RegisterType<EntityFrameworkEventRepository>().As<IEventRepository>();
        }
    }
}
