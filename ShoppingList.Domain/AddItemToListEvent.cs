using System;
using Lily.Core.Domain.Model;
using Newtonsoft.Json;

namespace Lily.ShoppingList.Domain
{
    [Serializable]
    public class AddItemToListEvent : Event, IImuttable
    {
        public AddItemToListEvent(string username) : base(username)
        {
        }

        [JsonProperty(PropertyName = "productId")]
        public Guid ProductId { get; set; }
    }
}
