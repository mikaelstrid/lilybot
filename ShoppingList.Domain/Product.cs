using System.Collections.Generic;
using Lily.Core.Domain.Model;

namespace Lily.ShoppingList.Domain
{
    public class Product : AggregateRoot
    {
        internal Product() { }
        public Product(string username) : base(username) { }
        public string Name { get; set; }
        public virtual ICollection<StoreSection> StoreSections { get; set; }
        public virtual ICollection<Store> IgnoredInStores { get; set; }
    }
}
