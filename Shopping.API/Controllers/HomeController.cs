using System.Web.Http;

namespace Lilybot.Shopping.API.Controllers
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