using System.ComponentModel.DataAnnotations.Schema;
using Lily.Core.Domain.Model;
using Newtonsoft.Json.Linq;

namespace Lily.ShoppingList.Domain
{
    [NotMapped]
    public class RemoveItemFromListEvent : Event, IImuttable
    {
        public RemoveItemFromListEvent() { }

        public RemoveItemFromListEvent(Event baseEvent) : base(baseEvent) { }

        public RemoveItemFromListEvent(string username, int itemId) : base(username)
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
