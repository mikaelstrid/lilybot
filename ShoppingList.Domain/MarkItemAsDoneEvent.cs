using Lily.Core.Domain.Model;

namespace Lily.ShoppingList.Domain
{
    public class MarkItemAsDoneEvent : Event, IImuttable
    {
        public MarkItemAsDoneEvent(string username) : base(username)
        {
        }
    }
}
