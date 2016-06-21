using Lily.Core.Application;
using Lily.ShoppingList.Domain;

namespace Lily.ShoppingList.Application
{
    public interface IProfileRepository : IAggregateRepository<Profile>
    {
        Profile GetFriend(string username);
    }
}