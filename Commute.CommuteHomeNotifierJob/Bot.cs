using System;
using System.IO;
using System.Linq;
using Lilybot.Commute.Application;
using Lilybot.Commute.CommuteHomeNotifierJob.Models;
using Lilybot.Positioning.CommonTypes;
using Lilybot.Positioning.CommonTypes.Extensions;

namespace Lilybot.Commute.CommuteHomeNotifierJob
{
    public interface IBot
    {
        void ProcessHotspotEvent(HotspotEventMessage ev, TextWriter log);
    }

    public class Bot : IBot
    {
        private readonly IFamilyProfileRepository _familyProfileRepository;
        private readonly IStateRepository _stateRepository;
        private readonly ISlackMessageSender _slackMessageSender;

        public Bot(IFamilyProfileRepository familyProfileRepository, IStateRepository stateRepository, ISlackMessageSender slackMessageSender)
        {
            _slackMessageSender = slackMessageSender;
            _familyProfileRepository = familyProfileRepository;
            _stateRepository = stateRepository;
        }

        public void ProcessHotspotEvent(HotspotEventMessage ev, TextWriter log)
        {
            if (ev.Timestamp.DayOfWeek == DayOfWeek.Saturday || ev.Timestamp.DayOfWeek == DayOfWeek.Sunday) return;

            var familyProfile = _familyProfileRepository.GetByFacebookUserId(ev.FacebookUserId);
            var userProfile = familyProfile.GetUser(ev.FacebookUserId);
            var familyState = _stateRepository.GetState(familyProfile.Id) ?? new FamilyState(familyProfile);

            var memberState = familyState.GetMemberState(ev.FacebookUserId);
            if (ev.HotspotType == HotspotType.Home && ev.ActionType == ActionType.Leave)
            {
                familyState.SetMemberState(ev.FacebookUserId,
                    ev.Timestamp.IsBefore(userProfile.LatestTimeForSchoolDropOff)
                        ? MemberState.OnWayToWork
                        : MemberState.Unknown, ev.Timestamp);
            }
            else if (ev.HotspotType == HotspotType.Work && ev.ActionType == ActionType.Enter)
            {
                familyState.SetMemberState(ev.FacebookUserId, MemberState.AtWork, ev.Timestamp);
            }
            else if (ev.HotspotType == HotspotType.Work && ev.ActionType == ActionType.Leave)
            {
                if (ev.Timestamp.IsBefore(userProfile.EarliestTimeForSchoolPickup)) return; // Probably left for lunch


                familyState.SetMemberState(ev.FacebookUserId, MemberState.OnWayHome, ev.Timestamp);
                if (!familyState.EventsThatTriggeredSlackMessages.Any())
                {
                    if (ev.Timestamp.IsAfter(userProfile.LatestTimeForSchoolPickup) ||
                        familyState.GetOtherMemberStates(ev.FacebookUserId).Any(s => s == MemberState.AtHome || s == MemberState.Unknown || s == MemberState.OnWayHome))
                    {
                        _slackMessageSender.SendToSlack(CreateMessage(ev), log);
                        familyState.EventsThatTriggeredSlackMessages.Add(ev);
                    }
                }
            }
            else if (ev.HotspotType == HotspotType.Home && ev.ActionType == ActionType.Enter)
            {
                familyState.SetMemberState(ev.FacebookUserId, MemberState.AtHome, ev.Timestamp);
            }

            _stateRepository.SaveState(familyState);
        }

        private static string CreateMessage(HotspotEventMessage hotspotEvent)
        {
            switch (hotspotEvent.ActionType)
            {
                case ActionType.Enter:
                    return $"Mikael anlände till {hotspotEvent.HotspotName} kl {hotspotEvent.Timestamp.ToString("HH:mm")}";
                case ActionType.Leave:
                    return $"Mikael lämnade {hotspotEvent.HotspotName} kl {hotspotEvent.Timestamp.ToString("HH:mm")}";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
