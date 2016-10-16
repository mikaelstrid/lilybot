using System;

namespace Lilybot.Positioning.API.Extensions
{
    public static class DateTimeExtensions
    {
        // ReSharper disable once InconsistentNaming
        public static DateTimeOffset ToDateTimeOffsetWEST(this DateTime dateTime)
        {
            return new DateTimeOffset(dateTime, TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time").GetUtcOffset(dateTime));
        }
    }
}
