using System;

namespace Lilybot.Positioning.CommonTypes
{
    public class MessageBase
    {
        public MessageBase() { }

        public MessageBase(DateTimeOffset timestamp, string facebookUserId)
        {
            Timestamp = timestamp;
            FacebookUserId = facebookUserId;
        }

        public int Version { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string FacebookUserId { get; set; }
    }
}
