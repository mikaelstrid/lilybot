using Lilybot.Core.Application;
using Lilybot.Shopping.Domain;

namespace Lilybot.Shopping.Application
{
    public interface IProfileRepository : IAggregateRepository<ShoppingProfile>
    {
        ShoppingProfile GetFriend(string username);
    }
}