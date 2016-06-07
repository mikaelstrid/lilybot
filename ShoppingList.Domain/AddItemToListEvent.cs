using Lily.Core.Domain.Model;

namespace Lily.ShoppingList.Domain
{
    public class AddItemToListEvent : Event, IImuttable
    {
        public AddItemToListEvent(string username) : base(username)
        {
        }
    }
}
