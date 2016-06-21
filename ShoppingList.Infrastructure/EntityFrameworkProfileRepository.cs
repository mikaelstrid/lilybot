using System.Data.Entity;
using System.Linq;
using Lily.Core.Infrastructure.Persistence.EntityFramework;
using Lily.ShoppingList.Application;
using Lily.ShoppingList.Domain;

namespace Lily.ShoppingList.Infrastructure
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
