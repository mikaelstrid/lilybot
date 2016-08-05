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
        private readonly IAggregateRepository<CommuteProfile> _repository;

        public ProfilesController(IAggregateRepository<CommuteProfile> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [Route("me")]
        public IHttpActionResult GetMyProfile()
        {
            var existingProfile = _repository.Get(User.Identity.Name, p => true).SingleOrDefault();

            if (existingProfile != null) return Ok(existingProfile);

            var newProfile = new CommuteProfile(User.Identity.Name);
            _repository.InsertOrUpdate(User.Identity.Name, newProfile);
            return Ok(newProfile);
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Post()
        {
            if (_repository.Get(User.Identity.Name, p => true).Any()) return BadRequest($"A profile for user {User.Identity.Name} already exists.");
            var newProfile = new CommuteProfile(User.Identity.Name);
            _repository.InsertOrUpdate(User.Identity.Name, newProfile);
            return Ok(newProfile);
        }
    }
}
