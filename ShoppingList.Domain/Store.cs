using System.Collections.Generic;
using Lilybot.Core.Domain.Model;

namespace Lilybot.Shopping.Domain
{
    public class Store : AggregateRoot
    {
        internal Store()
        {
            Sections = new List<StoreSection>();
            IgnoredProducts = new List<Product>();
        }

        public Store(string username) : base(username)
        {
            Sections = new List<StoreSection>();
            IgnoredProducts = new List<Product>();
        }

        public string Name { get; set; }

        public virtual IList<StoreSection> Sections { get; set; }

        public virtual ICollection<Product> IgnoredProducts { get; set; }
    }

    public class StoreSection : Entity<int>
    {
        public StoreSection()
        {
            Products = new List<Product>();
        }

        public string Name { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public int Order { get; set; }
    }
}
