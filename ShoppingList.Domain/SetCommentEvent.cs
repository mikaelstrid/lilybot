using System.ComponentModel.DataAnnotations.Schema;
using Lily.Core.Domain.Model;
using Newtonsoft.Json.Linq;

namespace Lily.ShoppingList.Domain
{
    [NotMapped]
    public class SetCommentEvent : Event, IImuttable
    {
        public SetCommentEvent() { }

        public SetCommentEvent(Event baseEvent) : base(baseEvent) { }

        public SetCommentEvent(string username, int itemId, string comment) : base(username)
        {
            UpdatePayload(itemId, comment);
        }

        public int ItemId => JObject.Parse(Payload)["itemId"].Value<int>();

        public string Comment => JObject.Parse(Payload)["comment"].Value<string>();

        private void UpdatePayload(int itemId, string comment)
        {
            Payload = JObject.FromObject(
                new
                {
                    itemId,
                    comment
                })
                .ToString();

        }
    }
}
