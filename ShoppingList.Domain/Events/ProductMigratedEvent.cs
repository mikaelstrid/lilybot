using System;
using System.ComponentModel.DataAnnotations.Schema;
using Lily.Core.Domain.Model;
using Newtonsoft.Json.Linq;

namespace Lily.ShoppingList.Domain.Events
{
    [NotMapped]
    public class ProductMigratedEvent : Event, IImuttable
    {
        public ProductMigratedEvent() { }

        public ProductMigratedEvent(Event baseEvent) : base(baseEvent) { }

        public ProductMigratedEvent(string username, int productId, int count, DateTime countUpdateTimestampUtc) : base(username)
        {
            UpdatePayload(productId, count, countUpdateTimestampUtc);
        }

        public int ProductId => JObject.Parse(Payload)["productId"].Value<int>();

        private void UpdatePayload(int productId, int count, DateTime countUpdateTimestampUtc)
        {
            Payload = JObject.FromObject(
                new
                {
                    productId,
                    count,
                    countUpdated = countUpdateTimestampUtc
                })
                .ToString();
        }
    }
}
