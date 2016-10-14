using System;
using System.Globalization;
using System.Net;
using System.Web.Http;
using Lilybot.Positioning.Infrastructure;
using RestSharp;

namespace Lilybot.Positioning.API.Controllers
{
    [RoutePrefix("api/ifttt")]
    public class IftttController : ApiController
    {
        [Route("hotspot/{hotspotName}/enter")]
        [HttpPost]
        [BodyApiKeyAuthorize]
        public IHttpActionResult PostHotspotEnter(string hotspotName, [FromBody] HotspotApiModel model)
        {
            var occuredAt = ParseSlackDateTime(model.OccuredAt);
            var message = $"Mikael anlände till {hotspotName} kl {occuredAt.ToString("HH:mm")}";
            var response = PostToSlack(message);
            if (response.StatusCode == HttpStatusCode.OK)
                return Ok(message);
            else
                throw new Exception($"Error communicating with Slack, code {response.StatusCode}");
        }

        [Route("hotspot/{hotspotName}/leave")]
        [HttpPost]
        [BodyApiKeyAuthorize]
        public IHttpActionResult PostHotspotLeave(string hotspotName, [FromBody] HotspotApiModel model)
        {
            var occuredAt = ParseSlackDateTime(model.OccuredAt);
            var message = $"Mikael lämnade {hotspotName} kl {occuredAt.ToString("HH:mm")}";
            var response = PostToSlack(message);
            if (response.StatusCode == HttpStatusCode.OK)
                return Ok(message);
            else
                throw new Exception($"Error communicating with Slack, code {response.StatusCode}");
        }


        private static DateTime ParseSlackDateTime(string slackDateTime)
        {
            return DateTime.ParseExact(slackDateTime, "MMMM dd, yyyy 'at' h:mmtt", new CultureInfo("en-US"));
        }

        private static IRestResponse PostToSlack(string message)
        {
            var client = new RestClient("https://hooks.slack.com/services/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");
            var request = new RestRequest(Method.POST);
            request.AddJsonBody(new { text = message });
            return client.Execute(request);
        }
    }


    public class HotspotApiModel
    {
        public string OccuredAt { get; set; }
    }
}
