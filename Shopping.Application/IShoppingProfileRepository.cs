using Lilybot.Core.Application;
using Lilybot.Shopping.Domain;

namespace Lilybot.Shopping.Application
{
    public interface IShoppingProfileRepository : IAggregateRepository<ShoppingProfile>
    {
        ShoppingProfile GetFriend(string username);
    }
}