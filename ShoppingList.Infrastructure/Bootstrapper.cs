using System;
using Autofac;
using Lily.Core.Application;
using Lily.Core.Infrastructure.Persistence;
using Lily.ShoppingList.Domain;

namespace Lily.ShoppingList.Infrastructure
{
    public class Bootstrapper
    {
        public static void Bootstrap(ContainerBuilder builder)
        {
            builder.RegisterType<DocumentDbAggregateRepository<Product>>().As<IAggregateRepository<Product>>();
        }
    }
}
