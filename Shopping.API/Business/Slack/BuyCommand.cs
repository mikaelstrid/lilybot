using System.Linq;
using Lilybot.Core.Application;
using Lilybot.Shopping.Application;
using Lilybot.Shopping.API.ApiModels;
using Lilybot.Shopping.Domain;
using Lilybot.Shopping.Domain.Events;

namespace Lilybot.Shopping.API.Business.Slack
{
    public class BuyCommand
    {
        private readonly IAggregateRepository<Product> _productRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IItemsService _itemsService;

        public BuyCommand(IAggregateRepository<Product> productRepository, IEventRepository eventRepository, IItemsService itemsService)
        {
            _productRepository = productRepository;
            _eventRepository = eventRepository;
            _itemsService = itemsService;
        }

        public string Handle(SlackCommand cmd, ShoppingProfile profile)
        {
            if (string.IsNullOrWhiteSpace(cmd.text))
                return "Du måste skriva namnet på en vara, till exempel '/köp mjölk'.";

            if (cmd.text.Length <= 1)
                return "Du måste skriva ett lite längre namn på varan (minst två tecken).";

            // === EXACT MATCH ===
            var productsExactlyMatchingSearchTerm = _productRepository
                .Get(
                    profile.Username,
                    p => p.Name.ToLower() == cmd.text.ToLower())
                .ToList();

            if (productsExactlyMatchingSearchTerm.Any())
            {
                var product = productsExactlyMatchingSearchTerm.First();
                return AddNewItem(profile, product);
            }


            // === STARTING WITH ===
            var productsStartingWithSearchTerm = _productRepository
                .Get(
                    profile.Username,
                    p => p.Name.ToLower().StartsWith(cmd.text.ToLower()))
                .ToList();

            if (productsStartingWithSearchTerm.Count > 1)
            {
                return $"Jag hittar flera produkter som börjar med '{cmd.text}', till exempel {string.Join(", ", productsStartingWithSearchTerm.Take(5).Select(p => p.Name))}, ge mig några tecken till.";
            }
            if (productsStartingWithSearchTerm.Count == 1)
            {
                var product = productsStartingWithSearchTerm.First();
                return  AddNewItem(profile, product);
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
                return $"Jag hittar inga produkter som innehåller '{cmd.text}'.";
            }
            if (productsContainingSearchTerm.Count > 1)
            {
                return $"Jag hittar flera produkter som innehåller '{cmd.text}', till exempel {string.Join(", ", productsContainingSearchTerm.Take(5).Select(p => p.Name))}, ge mig några tecken till.";
            }
            else
            {
                var product = productsContainingSearchTerm.First();
                return AddNewItem(profile, product);
            }
        }

        private string AddNewItem(ShoppingProfile profile, Product product)
        {
            if (_itemsService.GetItems(profile.Username).Where(i => i.Active).Any(i => i.ProductId == product.Id))
                return $"{product.Name} finns redan i inköpslistan.";

            var newEvent = new ItemAddedToListEvent(profile.Username, product.Id);
            _eventRepository.Insert(profile.Username, newEvent);
            return $"{product.Name} tillagt i inköpslistan.";
        }

    }
}