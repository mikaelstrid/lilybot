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
            //builder.RegisterInstance(new DocumentDbProfileRepository("ShoppingListProfileCollection")).As<IProfileRepository>();
            //builder.RegisterInstance(new EntityFrameworkAggregateRepository<Product>("ShoppingListProductCollection")).As<IAggregateRepository<Product>>();
            //builder.RegisterInstance(new DocumentDbAggregateRepository<Store>("ShoppingListStoreCollection")).As<IAggregateRepository<Store>>();
            //builder.RegisterInstance(new DocumentDbAggregateRepository<AddItemToListEvent>("ShoppingListEventCollection")).As<IAggregateRepository<AddItemToListEvent>>();
            //builder.RegisterInstance(new DocumentDbAggregateRepository<ReAddItemToListEvent>("ShoppingListEventCollection")).As<IAggregateRepository<ReAddItemToListEvent>>();
            //builder.RegisterInstance(new DocumentDbAggregateRepository<RemoveItemFromListEvent>("ShoppingListEventCollection")).As<IAggregateRepository<RemoveItemFromListEvent>>();
            //builder.RegisterInstance(new DocumentDbAggregateRepository<MarkItemAsDoneEvent>("ShoppingListEventCollection")).As<IAggregateRepository<MarkItemAsDoneEvent>>();
            //builder.RegisterInstance(new DocumentDbAggregateRepository<SetCommentEvent>("ShoppingListEventCollection")).As<IAggregateRepository<SetCommentEvent>>();

            builder.RegisterInstance(new ShoppingListDbContext()).As<DbContext>();

            builder.RegisterType<EntityFrameworkProfileRepository>().As<IProfileRepository>();
            builder.RegisterType<EntityFrameworkAggregateRepository<Product>>().As<IAggregateRepository<Product>>();
            //builder.RegisterType<EntityFrameworkAggregateRepository<Store>>().As<IAggregateRepository<Store>>();
            //builder.RegisterType<EntityFrameworkAggregateRepository<AddItemToListEvent>>().As<IAggregateRepository<AddItemToListEvent>>();
            //builder.RegisterType<EntityFrameworkAggregateRepository<ReAddItemToListEvent>>().As<IAggregateRepository<ReAddItemToListEvent>>();
            //builder.RegisterType<EntityFrameworkAggregateRepository<RemoveItemFromListEvent>>().As<IAggregateRepository<RemoveItemFromListEvent>>();
            //builder.RegisterType<EntityFrameworkAggregateRepository<MarkItemAsDoneEvent>>().As<IAggregateRepository<MarkItemAsDoneEvent>>();
            //builder.RegisterType<EntityFrameworkAggregateRepository<SetCommentEvent>>().As<IAggregateRepository<SetCommentEvent>>();
        }
    }
}
