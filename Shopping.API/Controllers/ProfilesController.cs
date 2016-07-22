using System.Linq;
using System.Web.Http;
using Lilybot.Shopping.Application;
using Lilybot.Shopping.Domain;

namespace Lilybot.Shopping.API.Controllers
{
    [Authorize]
    [RoutePrefix("api/profiles")]
    public class ProfilesController : ApiController
    {
        private readonly IProfileRepository _repository;

        public ProfilesController(IProfileRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Post()
        {
            if (_repository.Get(User.Identity.Name, p => true).Any()) return BadRequest($"A profile for user {User.Identity.Name} already exists.");
            var newProfile = new ShoppingProfile(User.Identity.Name);
            _repository.InsertOrUpdate(User.Identity.Name, newProfile);
            return Ok(newProfile);
        }

        [HttpPost]
        [Route("friends")]
        public IHttpActionResult PostFriend([FromBody] AddFriendApiModel model)
        {
            var parentProfile = _repository.Get(User.Identity.Name, p => true).SingleOrDefault();
            if (parentProfile == null) return BadRequest($"There is no profile for parent user {User.Identity.Name}.");

            parentProfile.Friends += model.FriendUsername + ";";
            _repository.InsertOrUpdate(User.Identity.Name, parentProfile);
            return Ok();
        }

        [HttpDelete]
        [Route("friends/{username}")]
        public IHttpActionResult DeleteFriend(string username)
        {
            var parentProfile = _repository.Get(User.Identity.Name, p => true).SingleOrDefault();
            if (parentProfile == null) return BadRequest($"There is no profile for parent user {User.Identity.Name}.");

            if (!parentProfile.Friends.Contains(username)) return BadRequest($"No friend profile {username} found.");

            parentProfile.Friends = parentProfile.Friends.Replace(username + ";", "");
            _repository.InsertOrUpdate(User.Identity.Name, parentProfile);
            return Ok();
        }
    }

    public class AddFriendApiModel
    {
        public string FriendUsername { get; set; }
    }
}
