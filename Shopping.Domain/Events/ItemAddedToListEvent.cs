using System.ComponentModel.DataAnnotations.Schema;
using Lilybot.Core.Domain.Model;
using Newtonsoft.Json.Linq;

namespace Lilybot.Shopping.Domain.Events
{
    [NotMapped]
    public class ItemAddedToListEvent : Event, IImuttable
    {
        public ItemAddedToListEvent() { }

        public ItemAddedToListEvent(Event baseEvent) : base(baseEvent) { }

        public ItemAddedToListEvent(string username, int productId) : base(username)
        {
            UpdatePayload(productId);
        }

        public int ProductId => JObject.Parse(Payload)["productId"].Value<int>();

        private void UpdatePayload(int productId)
        {
            Payload = JObject.FromObject(
                new
                {
                    productId
                })
                .ToString();
        }
    }
}
