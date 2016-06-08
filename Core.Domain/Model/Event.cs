using System;
using Newtonsoft.Json;

namespace Lily.Core.Domain.Model
{
    [Serializable]
    public class Event : AggregateRoot
    {
        internal Event() { }

        public Event(string username) : base(username, Guid.NewGuid())
        {
            TimestampUtc = DateTime.UtcNow;
        }

        [JsonProperty(PropertyName = "timestampUtc")]
        public DateTime TimestampUtc { get; internal set; }
    }
}