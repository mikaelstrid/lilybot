using System.ComponentModel.DataAnnotations.Schema;
using Lilybot.Core.Domain.Model;
using Newtonsoft.Json.Linq;

namespace Lilybot.Shopping.Domain.Events
{
    [NotMapped]
    public class CommentSetOnItemEvent : Event, IImuttable
    {
        public CommentSetOnItemEvent() { }

        public CommentSetOnItemEvent(Event baseEvent) : base(baseEvent) { }

        public CommentSetOnItemEvent(string username, int itemId, string comment) : base(username)
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
