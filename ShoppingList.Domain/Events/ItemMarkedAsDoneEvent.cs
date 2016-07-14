using System.ComponentModel.DataAnnotations.Schema;
using Lilybot.Core.Domain.Model;
using Newtonsoft.Json.Linq;

namespace Lilybot.Shopping.Domain.Events
{
    [NotMapped]
    public class ItemMarkedAsDoneEvent : Event, IImuttable
    {
        public ItemMarkedAsDoneEvent() { }

        public ItemMarkedAsDoneEvent(Event baseEvent) : base(baseEvent) { }

        public ItemMarkedAsDoneEvent(string username, int itemId) : base(username)
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
