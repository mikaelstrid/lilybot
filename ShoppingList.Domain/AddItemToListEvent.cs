using System.ComponentModel.DataAnnotations.Schema;
using Lily.Core.Domain.Model;
using Newtonsoft.Json.Linq;

namespace Lily.ShoppingList.Domain
{
    [NotMapped]
    public class AddItemToListEvent : Event, IImuttable
    {
        public AddItemToListEvent() { }

        public AddItemToListEvent(Event baseEvent) : base(baseEvent) { }

        public AddItemToListEvent(string username, int productId) : base(username)
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
