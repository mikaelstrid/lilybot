using System;
using Lilybot.Core.Domain.Model;

namespace Lilybot.Commute.Domain
{
    public class UserProfile : Entity<string>
    {
        public TimeSpan LatestTimeForSchoolDropOff { get; set; }
        public TimeSpan LatestTimeForSchoolPickup { get; set; }
        public TimeSpan EarliestTimeForSchoolPickup { get; set; }
    }
}
