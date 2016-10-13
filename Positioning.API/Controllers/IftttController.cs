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
        public IHttpActionResult PostHotspotEnter(string hotspotName, [FromBody] HotspotEnterApiModel model)
        {
            var occuredAt = DateTime.ParseExact(model.OccuredAt, "MMMM dd, yyyy 'at' h:mmtt", new CultureInfo("en-US"));
            var message = $"Mikael @ {hotspotName} kl {occuredAt.ToString("HH:mm")}";
            var response = PostToSlack(message);
            if (response.StatusCode == HttpStatusCode.OK)
                return Ok(message);
            else
                throw new Exception($"Error communicating with Slack, code {response.StatusCode}");
        }

        private static IRestResponse PostToSlack(string message)
        {
            var client = new RestClient("https://hooks.slack.com/services/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");
            var request = new RestRequest(Method.POST);
            request.AddJsonBody(new { text = message });
            return client.Execute(request);
        }
    }


    public class HotspotEnterApiModel
    {
        public string OccuredAt { get; set; }
    }
}
