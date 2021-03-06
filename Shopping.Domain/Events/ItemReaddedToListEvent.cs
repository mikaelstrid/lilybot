using System.ComponentModel.DataAnnotations.Schema;
using Lilybot.Core.Domain.Model;
using Newtonsoft.Json.Linq;

namespace Lilybot.Shopping.Domain.Events
{
    [NotMapped]
    public class ItemReaddedToListEvent : Event, IImuttable
    {
        public ItemReaddedToListEvent() { }

        public ItemReaddedToListEvent(Event baseEvent) : base(baseEvent) { }

        public ItemReaddedToListEvent(string username, int oldItemId) : base(username)
        {
            UpdatePayload(oldItemId);
        }

        public int OldItemId => JObject.Parse(Payload)["oldItemId"].Value<int>();

        private void UpdatePayload(int oldItemId)
        {
            Payload = JObject.FromObject(
                new
                {
                    oldItemId
                })
                .ToString();

        }
    }
}
