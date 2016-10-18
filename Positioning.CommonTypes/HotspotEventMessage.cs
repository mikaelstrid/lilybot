using System;

namespace Lilybot.Positioning.CommonTypes
{
    public class HotspotEventMessage : MessageBase
    {
        public HotspotEventMessage() { }

        public HotspotEventMessage(DateTimeOffset timestamp, string facebookUserId, string hotspotName, ActionType actionType) : base(timestamp, facebookUserId)
        {
            Version = 1;
            HotspotName = hotspotName;
            ActionType = actionType;
        }

        public string HotspotName { get; set; }
        public ActionType ActionType { get; set; }
    }

    public enum ActionType
    {
        Enter = 0,
        Leave = 1,
    }
}
