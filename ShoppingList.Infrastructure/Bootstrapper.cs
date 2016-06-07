using Autofac;
using Lily.Core.Application;
using Lily.Core.Domain.Model;
using Lily.Core.Infrastructure.Persistence;
using Lily.ShoppingList.Domain;

namespace Lily.ShoppingList.Infrastructure
{
    public class Bootstrapper
    {
        public static void Bootstrap(ContainerBuilder builder)
        {
            builder.RegisterInstance(new DocumentDbAggregateRepository<Product>("ShoppingListProductCollection")).As<IAggregateRepository<Product>>();
            builder.RegisterInstance(new DocumentDbAggregateRepository<Store>("ShoppingListStoreCollection")).As<IAggregateRepository<Store>>();
            builder.RegisterInstance(new DocumentDbAggregateRepository<Event>("ShoppingListEventCollection")).As<IAggregateRepository<Event>>();
        }
    }
}
