using System;
using System.Collections.Generic;
using Lilybot.Core.Domain.Model;

namespace Lilybot.Shopping.Domain
{
    public class Product : AggregateRoot
    {
        internal Product() { }
        public Product(string username) : base(username) { }
        public string Name { get; set; }
        public int Count { get; set; }
        public DateTime CountUpdateTimestampUtc { get; set; }
        public virtual ICollection<StoreSection> StoreSections { get; set; }
        public virtual ICollection<Store> IgnoredInStores { get; set; }
    }
}
