using System;
using Lily.Core.Domain.Model;
using Newtonsoft.Json;

namespace Lily.ShoppingList.Domain
{
    public class MarkItemAsDoneEvent : Event, IImuttable
    {
        public MarkItemAsDoneEvent(string username) : base(username)
        {
        }

        [JsonProperty(PropertyName = "itemId")]
        public Guid ItemId { get; set; }
    }
}
