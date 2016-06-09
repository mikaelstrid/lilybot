using System;
using Lily.Core.Domain.Model;
using Newtonsoft.Json;

namespace Lily.ShoppingList.Domain
{
    [Serializable]
    public class ReAddItemToListEvent : Event, IImuttable
    {
        public ReAddItemToListEvent(string username) : base(username)
        {
        }

        [JsonProperty(PropertyName = "oldItemId")]
        public Guid OldItemId { get; set; }
    }
}
