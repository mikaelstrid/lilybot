using System.Web.Http;
using Lilybot.Shopping.Infrastructure;

namespace Lilybot.Shopping.API.Controllers
{
    [Route("api/slack")]
    [SlackTokenAuthorize]
    public class SlackController : ApiController
    {
        [HttpPost]
        public IHttpActionResult Post([FromBody] SlackCommand cmd)
        {
            return Ok();
        }
    }

    public class SlackCommand
    {
        // ReSharper disable InconsistentNaming
        public string token { get; set; }
        public string team_id { get; set; }
        public string team_domain { get; set; }
        public string channel_id { get; set; }
        public string channel_name { get; set; }
        public string user_id { get; set; }
        public string user_name { get; set; }
        public string command { get; set; }
        public string text { get; set; }
        public string response_url { get; set; }
    }
}
