using Lily.Core.Application;
using Lily.ShoppingList.Domain;

namespace Lily.ShoppingList.Application
{
    public interface IStoreRepository : IAggregateRepository<Store>
    {
        void DeleteSectionById(string username, int storeId, int sectionId);
    }
}