using Lilybot.Core.Application;
using Lilybot.Shopping.Domain;

namespace Lilybot.Shopping.Application
{
    public interface IProfileRepository : IAggregateRepository<Profile>
    {
        Profile GetFriend(string username);
    }
}