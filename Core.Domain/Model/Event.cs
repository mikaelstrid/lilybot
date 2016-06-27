using System;

namespace Lily.Core.Domain.Model
{
    public class Event : Entity<int>
    {
        // Used by Entity Framework
        protected Event() { }

        // Used when deserialing from the database
        protected internal Event(Event baseEvent)
        {
            Id = baseEvent.Id;
            Username = baseEvent.Username;
            TimestampUtc = baseEvent.TimestampUtc;
            EventType = baseEvent.EventType;
            Payload = baseEvent.Payload;
        }

        // Used by the repository
        public Event(string username)
        {
            Username = username;
            TimestampUtc = DateTime.UtcNow;
            EventType = GetType().AssemblyQualifiedName;
        }

        public string Username { get; internal set; }
        public DateTime TimestampUtc { get; internal set; }
        public string EventType { get; internal set; }
        public string Payload { get; protected internal set; }

        internal Event ToDerivedType()
        {
            var type = Type.GetType(EventType);
            return (Event) Activator.CreateInstance(type, this);
        }
        internal Event ToBaseType()
        {
            return new Event(this);    
        }
    }
}