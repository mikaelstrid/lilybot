using System;
using System.Globalization;
using System.Web.Http;
using Lilybot.Positioning.API.Extensions;
using Lilybot.Positioning.CommonTypes;
using Lilybot.Positioning.Infrastructure;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;

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
            SendToServiceBusTopic(new HotspotEventMessage(
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

            topicClient.Send(new BrokeredMessage(JsonConvert.SerializeObject(message)));
        }
    }


    public class HotspotApiModel
    {
        public string OccuredAt { get; set; }
        public string FacebookUserId { get; set; }
    }
}
