using System;
using System.Collections.Generic;
using System.Linq;

namespace Lilybot.Commute.Domain
{
    public class FamilyProfile
    {
        public Guid Id { get; set; }
        public List<UserProfile> Adults { get; set; } = new List<UserProfile>();

        public UserProfile GetUser(string facebookUserId)
        {
            return Adults.Single(a => a.Id == facebookUserId);
        }
    }
}