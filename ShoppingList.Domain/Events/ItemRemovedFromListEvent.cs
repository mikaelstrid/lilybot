using System.ComponentModel.DataAnnotations.Schema;
using Lily.Core.Domain.Model;
using Newtonsoft.Json.Linq;

namespace Lily.ShoppingList.Domain.Events
{
    [NotMapped]
    public class ItemRemovedFromListEvent : Event, IImuttable
    {
        public ItemRemovedFromListEvent() { }

        public ItemRemovedFromListEvent(Event baseEvent) : base(baseEvent) { }

        public ItemRemovedFromListEvent(string username, int itemId) : base(username)
        {
            UpdatePayload(itemId);
        }

        public int ItemId => JObject.Parse(Payload)["itemId"].Value<int>();

        private void UpdatePayload(int itemId)
        {
            Payload = JObject.FromObject(
                new
                {
                    itemId
                })
                .ToString();

        }
    }
}
