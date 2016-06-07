using System;
using Newtonsoft.Json;

namespace Lily.Core.Domain.Model
{
    public class Event : AggregateRoot
    {
        internal Event() { }

        public Event(string username) : base(username, Guid.NewGuid())
        {
            TimestampUtc = DateTime.UtcNow;
        }

        public DateTime TimestampUtc { get; set; }
    }
}