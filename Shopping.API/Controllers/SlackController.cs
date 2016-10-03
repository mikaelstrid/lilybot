using System.Web.Http;
using Lilybot.Core.Application;
using Lilybot.Shopping.Application;
using Lilybot.Shopping.API.ApiModels;
using Lilybot.Shopping.API.Business.Slack;
using Lilybot.Shopping.Domain;
using Lilybot.Shopping.Infrastructure;
using Newtonsoft.Json;

namespace Lilybot.Shopping.API.Controllers
{
    [Route("api/slack")]
    [SlackTokenAuthorize]
    public class SlackController : ApiController
    {
        private readonly IShoppingProfileRepository _profileRepository;
        private readonly IAggregateRepository<Product> _productRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IItemsService _itemsService;

        public SlackController(IShoppingProfileRepository profileRepository, IAggregateRepository<Product> productRepository, IEventRepository eventRepository, IItemsService itemsService)
        {
            _profileRepository = profileRepository;
            _productRepository = productRepository;
            _eventRepository = eventRepository;
            _itemsService = itemsService;
        }

        [HttpPost]
        public IHttpActionResult Post([FromBody] SlackCommand cmd)
        {
            // Check if slack user exists in lilybot (and get username)
            var profile = _profileRepository.GetBySlackUserId(cmd.user_id);
            if (profile == null) return Ok("Du verkar inte ha något lilybot-konto ännu.");

            // Handle the command
            if (cmd.command.ToLower() == "/köp")
            {
                return Ok(new BuyCommand(_productRepository, _eventRepository).Handle(cmd, profile));
            }
            else if (cmd.command.ToLower() == "/inköpslista")
            {
                return Ok(new ListCommand(_itemsService).Handle(cmd, profile));
            }

            return Ok($"Jag kände inte igen kommandot '{cmd.command}'.");
        }
    }
}
