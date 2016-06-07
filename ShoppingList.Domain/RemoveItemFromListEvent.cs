using Lily.Core.Domain.Model;

namespace Lily.ShoppingList.Domain
{
    public class RemoveItemFromListEvent : Event, IImuttable
    {
        public RemoveItemFromListEvent(string username) : base(username)
        {
        }
    }
}
