using System.Web.Http;
using Lilybot.Positioning.Infrastructure;

namespace Lilybot.Positioning.API.Controllers
{
    [RoutePrefix("api/ifttt")]
    public class IftttController : ApiController
    {
        [Route("hotspot/enter")]
        [HttpPost]
        [BodyApiKeyAuthorize]
        public IHttpActionResult PostHotspotEnter([FromBody] HotspotEnterApiModel model)
        {
            return Ok();
        }
    }

    public class HotspotEnterApiModel
    {
    }
}
