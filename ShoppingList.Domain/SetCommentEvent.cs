using System;
using Lily.Core.Domain.Model;
using Newtonsoft.Json;

namespace Lily.ShoppingList.Domain
{
    public class SetCommentEvent : Event, IImuttable
    {
        public SetCommentEvent(string username) : base(username)
        {
        }

        [JsonProperty(PropertyName = "itemId")]
        public Guid ItemId { get; set; }

        [JsonProperty(PropertyName = "comment")]
        public string Comment { get; set; }
    }
}
