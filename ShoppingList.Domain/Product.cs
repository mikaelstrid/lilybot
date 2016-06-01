using System;
using Lily.Core.Domain.Model;
using Newtonsoft.Json;

namespace Lily.ShoppingList.Domain
{
    [Serializable]
    public class Product : Entity<Guid>, IAggregate
    {
        public Product() : this(Guid.NewGuid()) { }
        public Product(Guid guid) : base(guid) { }
        
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}
