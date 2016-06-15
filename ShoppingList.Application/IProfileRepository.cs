using System.Collections.Generic;
using System.Threading.Tasks;
using Lily.Core.Application;
using Lily.ShoppingList.Domain;

namespace Lily.ShoppingList.Application
{
    public interface IProfileRepository : IAggregateRepository<Profile>
    {
        Task<Profile> GetFriend(string username);
    }
}