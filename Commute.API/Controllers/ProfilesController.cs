using System.Linq;
using System.Web.Http;
using Lilybot.Commute.Domain;
using Lilybot.Core.Application;

namespace Lilybot.Commute.API.Controllers
{
    [Authorize]
    [RoutePrefix("api/profiles")]
    public class ProfilesController : ApiController
    {
        private readonly IAggregateRepository<Profile> _repository;

        public ProfilesController(IAggregateRepository<Profile> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [Route("me")]
        public IHttpActionResult GetMyProfile()
        {
            return Ok(new Profile(User.Identity.Name));
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Post()
        {
            if (_repository.Get(User.Identity.Name, p => true).Any()) return BadRequest($"A profile for user {User.Identity.Name} already exists.");
            var newProfile = new Profile(User.Identity.Name);
            _repository.InsertOrUpdate(User.Identity.Name, newProfile);
            return Ok(newProfile);
        }
    }
}
