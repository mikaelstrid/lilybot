using System.Data.Entity;
using Lilybot.Commute.Domain;

namespace Lilybot.Commute.Infrastructure
{
    public class CommuteDbContext : DbContext
    {
        public CommuteDbContext() : base("name=DefaultConnection") { }

        public DbSet<CommuteProfile> Profiles { get; set; }       
    }
}
