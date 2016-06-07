using System;
using System.Collections.Generic;
using Lily.Core.Domain.Model;
using Newtonsoft.Json;

namespace Lily.ShoppingList.Domain
{
    [Serializable]
    public class Store : AggregateRoot
    {
        internal Store() { }
        public Store(string username) : this(username, Guid.NewGuid()) { }
        public Store(string username, Guid guid) : base(username, guid) { }
        
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "sections")]
        public List<StoreSection> Sections { get; set; } = new List<StoreSection>();

        [JsonProperty(PropertyName = "ignoredProducts")]
        public StoreSection IgnoredProducts { get; set; } = new StoreSection() { Name = "Ignorerade produkter" };
    }

    [Serializable]
    public class StoreSection : Entity<Guid>
    {
        public StoreSection() : this(Guid.NewGuid()) { }
        public StoreSection(Guid id) : base(id) { }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "productIds")]
        public List<Guid> ProductIds { get; set; } = new List<Guid>();
    }
}
