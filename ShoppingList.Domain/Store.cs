using System.Collections.Generic;
using Lily.Core.Domain.Model;

namespace Lily.ShoppingList.Domain
{
    public class Store : AggregateRoot
    {
        public Store(string username) : base(username) { }
        
        public string Name { get; set; }

        public virtual IList<StoreSection> Sections { get; set; }

        public virtual StoreSection IgnoredProducts { get; set; }
    }

    public class StoreSection : Entity<int>
    {
        public string Name { get; set; }

        public IList<Product> Products { get; set; }
    }
}
