using System;

namespace Lily.Core.Domain.Model
{
    public class Event : Entity<int>
    {
        public Event(string username)
        {
            Username = username;
            TimestampUtc = DateTime.UtcNow;
        }

        public string Username { get; set; }

        public DateTime TimestampUtc { get; set; }
    }
}