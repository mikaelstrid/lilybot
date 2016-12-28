using System;
using System.Collections.Generic;
using System.Linq;
using Lilybot.Commute.Domain;
using Lilybot.Positioning.CommonTypes;

namespace Lilybot.Commute.CommuteHomeNotifierJob.Models
{
    public class FamilyState
    {
        public FamilyState() { }

        public FamilyState(FamilyProfile family)
        {
            family.Adults.ForEach(a => SetMemberState(a.Id, MemberState.Unknown, null));
        }

        public List<Member> Members { get; set; } = new List<Member>();
        public List<HotspotEventMessage> EventsThatTriggeredSlackMessages { get; set; } = new List<HotspotEventMessage>();

        public MemberState GetMemberState(string facebookUserId)
        {
            var existingMember = Members.SingleOrDefault(m => m.FacebookUserId == facebookUserId);
            return existingMember?.State ?? MemberState.Unknown;
        }

        public void SetMemberState(string facebookUserId, MemberState state, DateTimeOffset? timestamp)
        {
            var existingMember = Members.SingleOrDefault(m => m.FacebookUserId == facebookUserId);
            if (existingMember == null)
            {
                existingMember = new Member(facebookUserId);
                Members.Add(existingMember);
            }
            existingMember.State = state;
            existingMember.LastStateChange = timestamp;
        }
        
        public ICollection<MemberState> GetOtherMemberStates(string facebookUserId)
        {
            return Members.Where(m => m.FacebookUserId != facebookUserId).Select(m => m.State).ToArray();
        }


        public class Member
        {
            public Member(string facebookUserId, MemberState state = MemberState.Unknown, DateTimeOffset? timestamp = null)
            {
                FacebookUserId = facebookUserId;
                State = state;
                LastStateChange = timestamp;
            }

            public string FacebookUserId { get; set; }
            public MemberState State { get; set; }
            public DateTimeOffset? LastStateChange { get; set; }
        }
    }

    public enum MemberState
    {
        Unknown,
        OnWayToWork,
        AtWork,
        OnWayHome,
        AtHome
    }
}
