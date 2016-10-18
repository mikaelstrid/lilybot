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
        public Functions()
        {
                        
        }

        public void TopicListener([ServiceBusTrigger(topicName: "HotspotEvents", subscriptionName: "CommutingUpdates")] string message, TextWriter log)
        {
            try
            {
                var hotspotEventMessage = JsonConvert.DeserializeObject<HotspotEventMessage>(message);
                var messageToSlack = CreateMessage(hotspotEventMessage);
                log.WriteLine($"Sending message '{messageToSlack}' to Slack...");
                var sendResult = PostToSlack(messageToSlack);
                log.WriteLine(sendResult.StatusCode == HttpStatusCode.OK
                    ? "Message sent successfully."
                    : $"Sending message failed, status code: {sendResult.StatusCode}.");
            }
            catch (Exception e)
            {
                log.WriteLine("Exception caught when parsing message from topic. " + e);
            }
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

        private static IRestResponse PostToSlack(string message)
        {
            var client = new RestClient("https://hooks.slack.com/services/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");
            var request = new RestRequest(Method.POST);
            request.AddJsonBody(new {text = message});
            return client.Execute(request);
        }
    }
}
