using System.Data.Entity;
using System.Linq;
using Lilybot.Core.Infrastructure.Persistence.EntityFramework;
using Lilybot.Shopping.Application;
using Lilybot.Shopping.Domain;

namespace Lilybot.Shopping.Infrastructure
{
    public class EntityFrameworkProfileRepository : EntityFrameworkAggregateRepository<Profile>, IProfileRepository
    {
        public EntityFrameworkProfileRepository(DbContext context) : base(context) { }

        public Profile GetFriend(string username)
        {
            return Get(p => p.Friends.Contains(username)).FirstOrDefault();
        }
    }
}
