using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Lilybot.Core.Application;
using Lilybot.Shopping.Application;
using Lilybot.Shopping.Domain;
using Lilybot.Shopping.Domain.Events;
using Lilybot.Shopping.Infrastructure;

namespace Lilybot.Shopping.API.Controllers
{
    [Route("api/slack")]
    [SlackTokenAuthorize]
    public class SlackController : ApiController
    {
        private readonly IShoppingProfileRepository _profileRepository;
        private readonly IAggregateRepository<Product> _productRepository;
        private readonly IEventRepository _eventRepository;

        public SlackController(IShoppingProfileRepository profileRepository, IAggregateRepository<Product> productRepository, IEventRepository eventRepository)
        {
            _profileRepository = profileRepository;
            _productRepository = productRepository;
            _eventRepository = eventRepository;
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
                if (string.IsNullOrWhiteSpace(cmd.text))
                    return Ok("Du måste skriva namnet på en vara, till exempel '/köp mjölk'.");

                if (cmd.text.Length <= 1)
                    return Ok("Du måste skriva ett lite längre namn på varan (minst två tecken).");

                // === EXACT MATCH ===
                var productsExactlyMatchingSearchTerm = _productRepository
                    .Get(
                        profile.Username,
                        p => p.Name.ToLower() == cmd.text.ToLower())
                    .ToList();

                if (productsExactlyMatchingSearchTerm.Any())
                {
                    var product = productsExactlyMatchingSearchTerm.First();
                    AddNewItem(profile, product);
                    return Ok($"{product.Name} tillagd i inköpslistan.");
                }


                // === STARTING WITH ===
                var productsStartingWithSearchTerm = _productRepository
                    .Get(
                        profile.Username,
                        p => p.Name.ToLower().StartsWith(cmd.text.ToLower()))
                    .ToList();

                if (productsStartingWithSearchTerm.Count > 1)
                {
                    return Ok($"Jag hittar flera produkter som börjar med '{cmd.text}', till exempel {string.Join(", ", productsStartingWithSearchTerm.Take(5).Select(p => p.Name))}, ge mig några tecken till.");
                }
                if (productsStartingWithSearchTerm.Count == 1)
                {
                    var product = productsStartingWithSearchTerm.First();
                    AddNewItem(profile, product);
                    return Ok($"{product.Name} tillagd i inköpslistan.");
                }


                // === CONTAINS ===
                var productsContainingSearchTerm = _productRepository
                    .Get(
                        profile.Username,
                        predicate: p => p.Name.ToLower().Contains(cmd.text.ToLower()),
                        orderBy: o => o.OrderByDescending(p => p.Count))
                    .ToList();

                if (productsContainingSearchTerm.Count == 0)
                {
                    return Ok($"Jag hittar inga produkter som innehåller '{cmd.text}'.");
                }
                else if (productsContainingSearchTerm.Count > 1)
                {
                    return Ok($"Jag hittar flera produkter som innehåller '{cmd.text}', till exempel {string.Join(", ", productsContainingSearchTerm.Take(5).Select(p => p.Name))}, ge mig några tecken till.");
                }
                else
                {
                    var product = productsContainingSearchTerm.First();
                    AddNewItem(profile, product);
                    return Ok($"{product.Name} tillagd i inköpslistan.");
                }
            }

            return Ok($"Jag kände inte igen kommandot '{cmd.command}'.");
        }

        private void AddNewItem(ShoppingProfile profile, Product product)
        {
            var newEvent = new ItemAddedToListEvent(profile.Username, product.Id);
            _eventRepository.Insert(profile.Username, newEvent);
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
