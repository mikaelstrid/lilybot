using System.Data.Entity;
using Lily.ShoppingList.Domain;

namespace Lily.ShoppingList.Infrastructure
{
    public class ShoppingListDbContext : DbContext
    {
        public ShoppingListDbContext() : base("name=DefaultConnection") { }

        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Store> Stores { get; set; }
    }
}
