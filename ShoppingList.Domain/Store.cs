using System;
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
    }
}
