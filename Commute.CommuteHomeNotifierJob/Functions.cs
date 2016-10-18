using System;
using System.IO;
using System.Net;
using Lilybot.Positioning.CommonTypes;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using RestSharp;

namespace Lilybot.Commute.CommuteHomeNotifierJob
{
    public class Functions
    {
        private readonly IBot _bot;

        public Functions(IBot bot)
        {
            _bot = bot;
        }

        public void TopicListener([ServiceBusTrigger(topicName: "HotspotEvents", subscriptionName: "CommutingUpdates")] string message, TextWriter log)
        {
            try
            {
                var hotspotEventMessage = JsonConvert.DeserializeObject<HotspotEventMessage>(message);
                _bot.ProcessHotspotEvent(hotspotEventMessage, log);
            }
            catch (Exception e)
            {
                log.WriteLine("Exception caught when parsing message from topic. " + e);
            }
        }
    }
}
