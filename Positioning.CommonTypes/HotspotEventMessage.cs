using System;

namespace Lilybot.Positioning.CommonTypes
{
    public class HotspotEventMessage : MessageBase
    {
        public HotspotEventMessage() { }

        public HotspotEventMessage(DateTimeOffset timestamp, string facebookUserId, string hotspotName, HotspotType hotspotType, ActionType actionType) : base(timestamp, facebookUserId)
        {
            Version = 1;
            HotspotName = hotspotName;
            HotspotType = hotspotType;
            ActionType = actionType;
        }

        public string HotspotName { get; set; }
        public HotspotType HotspotType { get; set; }
        public ActionType ActionType { get; set; }
    }

    public enum HotspotType
    {
        Unknown = 0,
        Home = 1,
        Work = 2,
        Waypoint = 3,
        Other = 4
    }

    public enum ActionType
    {
        Enter = 0,
        Leave = 1,
    }
}
