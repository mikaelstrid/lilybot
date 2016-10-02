using System.Web.Http;

namespace Lilybot.Authentication.API.Controllers
{
    [RoutePrefix("")]
    public class HomeController : ApiController
    {
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            return Ok();
        }
    }
}