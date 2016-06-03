using System;
using System.Collections.Generic;
using Lily.Core.Domain.Model;
using Newtonsoft.Json;

namespace Lily.ShoppingList.Domain
{
    [Serializable]
    public class Store : Entity<Guid>, IAggregate
    {
        public Store() : this(Guid.NewGuid()) { }
        public Store(Guid guid) : base(guid) { }
        
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "sections")]
        public List<StoreSection> Sections { get; set; } = new List<StoreSection>();
    }

    [Serializable]
    public class StoreSection : Entity<Guid>
    {
        public StoreSection() : this(Guid.NewGuid()) { }
        public StoreSection(Guid id) : base(id) { }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}
