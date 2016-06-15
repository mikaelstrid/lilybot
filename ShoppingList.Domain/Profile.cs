using System;
using System.Collections.Generic;
using Lily.Core.Domain.Model;
using Newtonsoft.Json;

namespace Lily.ShoppingList.Domain
{
    [Serializable]
    public class Profile : AggregateRoot
    {
        internal Profile() { }
        public Profile(string username) : this(username, Guid.NewGuid()) { }
        public Profile(string username, Guid guid) : base(username, guid) { }

        [JsonProperty(PropertyName = "children")]
        public List<string> Children { get; set; }
    }
}
