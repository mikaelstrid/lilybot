using System;
using System.Globalization;
using System.Web.Http;
using Lilybot.Positioning.API.Extensions;
using Lilybot.Positioning.CommonTypes;
using Lilybot.Positioning.Infrastructure;
using Microsoft.ServiceBus.Messaging;
using RestSharp;

namespace Lilybot.Positioning.API.Controllers
{
    [RoutePrefix("api/ifttt")]
    public class IftttController : ApiController
    {
        [Route("hotspot/{hotspotName}/{actionType}")]
        [HttpPost]
        [BodyApiKeyAuthorize]
        public IHttpActionResult PostHotspotEnter(string hotspotName, string actionType, [FromBody] HotspotApiModel model)
        {
            SendToServiceBusTopic(new HotspotUpdateMessage(
                timestamp: ParseIftttDateTime(model.OccuredAt), 
                facebookUserId: model.FacebookUserId, 
                hotspotName: hotspotName, 
                actionType: ParseIftttActionType(actionType)));
            return Ok();
        }

        private static DateTimeOffset ParseIftttDateTime(string iftttDateTime)
        {
            return DateTime.ParseExact(iftttDateTime, "MMMM dd, yyyy 'at' h:mmtt", new CultureInfo("en-US")).ToDateTimeOffsetWEST();
        }

        private static ActionType ParseIftttActionType(string iftttActionType)
        {
            if (iftttActionType == "entered")
                return ActionType.Enter;
                //message = $"Mikael anlände till {hotspotName} kl {occuredAt.ToString("HH:mm")}";
            if (iftttActionType == "exited")
                return ActionType.Leave;
                //message = $"Mikael lämnade {hotspotName} kl {occuredAt.ToString("HH:mm")}";

            throw new ArgumentException($"'{iftttActionType}' is not a valid action.");
        }

        private static void SendToServiceBusTopic(MessageBase message)
        {
            var topicClient = TopicClient.CreateFromConnectionString(
                connectionString: "Endpoint=sb://lilybot.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=AF5s7IAJ+EPyQa8RfhFcTKAfrxCJJcuHcOsWS7hdP+I=", 
                path: "HotspotEvents");

            topicClient.Send(new BrokeredMessage(message));
        }

        //private static IRestResponse PostToSlack(string message)
        //{
        //    var client = new RestClient("https://hooks.slack.com/services/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");
        //    var request = new RestRequest(Method.POST);
        //    request.AddJsonBody(new { text = message });
        //    return client.Execute(request);
        //}
    }


    public class HotspotApiModel
    {
        public string OccuredAt { get; set; }
        public string FacebookUserId { get; set; }
    }
}
