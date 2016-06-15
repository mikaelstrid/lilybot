using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Lily.ShoppingList.Application;
using Lily.ShoppingList.Domain;

namespace Lily.ShoppingList.Api.Controllers
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
        public async Task<IHttpActionResult> Post()
        {
            if ((await _repository.Get(User.Identity.Name, p => true)).Any()) return BadRequest($"A profile for user {User.Identity.Name} already exists.");

            var newProfile = new Profile(User.Identity.Name);
            await _repository.AddOrUpdate(User.Identity.Name, newProfile);
            return Ok(newProfile);
        }

        [HttpPost]
        [Route("friends")]
        public async Task<IHttpActionResult> PostFriend([FromBody] AddFriendApiModel model)
        {
            var parentProfile = (await _repository.Get(User.Identity.Name, p => true)).SingleOrDefault();
            if (parentProfile == null) return BadRequest($"There is no profile for parent user {User.Identity.Name}.");

            parentProfile.Friends.Add(model.FriendUsername);
            await _repository.AddOrUpdate(User.Identity.Name, parentProfile);
            return Ok();
        }

        [HttpDelete]
        [Route("friends/{username}")]
        public async Task<IHttpActionResult> DeleteFriend(string username)
        {
            var parentProfile = (await _repository.Get(User.Identity.Name, p => true)).SingleOrDefault();
            if (parentProfile == null) return BadRequest($"There is no profile for parent user {User.Identity.Name}.");

            if (parentProfile.Friends.All(p => p != username)) return BadRequest($"No friend profile {username} found.");
            
            parentProfile.Friends.RemoveAll(p => p == username);
            await _repository.AddOrUpdate(User.Identity.Name, parentProfile);
            return Ok();
        }
    }

    public class AddFriendApiModel
    {
        public string FriendUsername { get; set; }
    }
}
