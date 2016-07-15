using Lilybot.Core.Application;
using Lilybot.Shopping.Domain;

namespace Lilybot.Shopping.Application
{
    public interface IStoreRepository : IAggregateRepository<Store>
    {
        void DeleteSectionById(string username, int storeId, int sectionId);
    }
}