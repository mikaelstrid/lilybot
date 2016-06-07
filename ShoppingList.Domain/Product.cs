using System;
using Lily.Core.Domain.Model;
using Newtonsoft.Json;

namespace Lily.ShoppingList.Domain
{
    [Serializable]
    public class Product : AggregateRoot
    {
        internal Product() { }
        public Product(string username) : this(username, Guid.NewGuid()) { }
        public Product(string username, Guid guid) : base(username, guid) { }
        
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}
