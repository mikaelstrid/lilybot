using System;

namespace Lilybot.Positioning.CommonTypes.Extensions
{
    public static class DateTimeOffsetExtensions
    {
        public static bool IsBefore(this DateTimeOffset dateTimeOffset, TimeSpan otherTime)
        {
            return new TimeSpan(dateTimeOffset.Hour, dateTimeOffset.Minute, dateTimeOffset.Second) < otherTime;
        }

        public static bool IsAfter(this DateTimeOffset dateTimeOffset, TimeSpan otherTime)
        {
            return new TimeSpan(dateTimeOffset.Hour, dateTimeOffset.Minute, dateTimeOffset.Second) > otherTime;
        }
    }
}