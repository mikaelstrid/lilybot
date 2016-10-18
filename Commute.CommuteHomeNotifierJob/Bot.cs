using System;
using System.IO;
using System.Net;
using Lilybot.Positioning.CommonTypes;

namespace Lilybot.Commute.CommuteHomeNotifierJob
{
    public interface IBot
    {
        void ProcessHotspotEvent(HotspotEventMessage hotspotEvent, TextWriter log);
    }

    public class Bot : IBot
    {
        private readonly ISlackMessageSender _slackMessageSender;
        private readonly IFamilyRepository _familyRepository;
        private readonly IHotspotEventRepository _eventRepository;

        public Bot(ISlackMessageSender slackMessageSender, IFamilyRepository familyRepository, IHotspotEventRepository eventRepository)
        {
            _slackMessageSender = slackMessageSender;
            _familyRepository = familyRepository;
            _eventRepository = eventRepository;
        }

        public void ProcessHotspotEvent(HotspotEventMessage hotspotEvent, TextWriter log)
        {
            //var family = _familyRepository.GetByFacebookUserId(hotspotEvent.FacebookUserId);

            var messageToSlack = CreateMessage(hotspotEvent);
            log.WriteLine($"Sending message '{messageToSlack}' to Slack...");
            var sendResult = _slackMessageSender.SendToSlack(messageToSlack);
            log.WriteLine(sendResult.StatusCode == HttpStatusCode.OK
                ? "Message sent successfully."
                : $"Sending message failed, status code: {sendResult.StatusCode}.");

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
