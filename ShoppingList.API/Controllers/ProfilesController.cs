using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Lily.Core.Application;
using Lily.ShoppingList.Domain;

namespace Lily.ShoppingList.Api.Controllers
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
        [Route("children")]
        public async Task<IHttpActionResult> PostChild([FromBody] AddChildApiModel model)
        {
            var parentProfile = (await _repository.Get(User.Identity.Name, p => true)).SingleOrDefault();
            if (parentProfile == null) return BadRequest($"There is no profile for parent user {User.Identity.Name}.");

            parentProfile.Children.Add(model.ChildUsername);
            await _repository.AddOrUpdate(User.Identity.Name, parentProfile);
            return Ok();
        }

        [HttpDelete]
        [Route("children/{username}")]
        public async Task<IHttpActionResult> DeleteChild(string username)
        {
            var parentProfile = (await _repository.Get(User.Identity.Name, p => true)).SingleOrDefault();
            if (parentProfile == null) return BadRequest($"There is no profile for parent user {User.Identity.Name}.");

            if (parentProfile.Children.All(p => p != username)) return BadRequest($"No child profile {username} found.");
            
            parentProfile.Children.RemoveAll(p => p == username);
            await _repository.AddOrUpdate(User.Identity.Name, parentProfile);
            return Ok();
        }
    }

    public class AddChildApiModel
    {
        public string ChildUsername { get; set; }
    }
}
