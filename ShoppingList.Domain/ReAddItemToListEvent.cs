using System.ComponentModel.DataAnnotations.Schema;
using Lily.Core.Domain.Model;
using Newtonsoft.Json.Linq;

namespace Lily.ShoppingList.Domain
{
    [NotMapped]
    public class ReAddItemToListEvent : Event, IImuttable
    {
        public ReAddItemToListEvent() { }

        public ReAddItemToListEvent(Event baseEvent) : base(baseEvent) { }

        public ReAddItemToListEvent(string username, int oldItemId) : base(username)
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
