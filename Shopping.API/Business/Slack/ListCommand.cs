using System.Linq;
using Lilybot.Shopping.Application;
using Lilybot.Shopping.API.ApiModels;
using Lilybot.Shopping.Domain;

namespace Lilybot.Shopping.API.Business.Slack
{
    public class ListCommand
    {
        private readonly IItemsService _itemsService;

        public ListCommand(IItemsService itemsService)
        {
            _itemsService = itemsService;
        }

        public SlackResponse Handle(SlackCommand cmd, ShoppingProfile profile)
        {
            var activeItems = _itemsService.GetItems(profile.Username).Where(i => i.Active).ToList();
            return activeItems.Any() 
                ? new SlackResponse("Här är din inköpslista: " + string.Join("\n", activeItems.Select(i => $"* {i.ProductName}"))) 
                : new SlackResponse("Det finns inget på inköpslistan just nu.");
        }
    }
}