using System.Data.Entity;
using Lily.Core.Domain.Model;
using Lily.ShoppingList.Domain;

namespace Lily.ShoppingList.Infrastructure
{
    public class ShoppingListDbContext : DbContext
    {
        public ShoppingListDbContext() : base("name=DefaultConnection") { }

        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Event> Events { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StoreSection>()
                .HasMany(s => s.Products).WithMany(p => p.StoreSections)
                .Map(t => t.MapLeftKey("StoreSectionId")
                    .MapRightKey("ProductId")
                    .ToTable("StoreSectionProducts"));

            modelBuilder.Entity<Store>()
                .HasMany(s => s.IgnoredProducts).WithMany(p => p.IgnoredInStores)
                .Map(t => t.MapLeftKey("StoreId")
                    .MapRightKey("ProductId")
                    .ToTable("IgnoredProducts"));
        }
    }
}
