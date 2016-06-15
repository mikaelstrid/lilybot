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
            builder.RegisterInstance(new DocumentDbAggregateRepository<Profile>("ShoppingListProfileCollection")).As<IAggregateRepository<Profile>>();
            builder.RegisterInstance(new DocumentDbAggregateRepository<Product>("ShoppingListProductCollection")).As<IAggregateRepository<Product>>();
            builder.RegisterInstance(new DocumentDbAggregateRepository<Store>("ShoppingListStoreCollection")).As<IAggregateRepository<Store>>();
            builder.RegisterInstance(new DocumentDbAggregateRepository<AddItemToListEvent>("ShoppingListEventCollection")).As<IAggregateRepository<AddItemToListEvent>>();
            builder.RegisterInstance(new DocumentDbAggregateRepository<ReAddItemToListEvent>("ShoppingListEventCollection")).As<IAggregateRepository<ReAddItemToListEvent>>();
            builder.RegisterInstance(new DocumentDbAggregateRepository<RemoveItemFromListEvent>("ShoppingListEventCollection")).As<IAggregateRepository<RemoveItemFromListEvent>>();
            builder.RegisterInstance(new DocumentDbAggregateRepository<MarkItemAsDoneEvent>("ShoppingListEventCollection")).As<IAggregateRepository<MarkItemAsDoneEvent>>();
            builder.RegisterInstance(new DocumentDbAggregateRepository<SetCommentEvent>("ShoppingListEventCollection")).As<IAggregateRepository<SetCommentEvent>>();
        }
    }
}
