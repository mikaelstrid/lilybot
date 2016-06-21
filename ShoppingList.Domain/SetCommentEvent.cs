using Lily.Core.Domain.Model;

namespace Lily.ShoppingList.Domain
{
    public class SetCommentEvent : Event, IImuttable
    {
        public SetCommentEvent(string username) : base(username) { }

        public int ItemId { get; set; }

        public string Comment { get; set; }
    }
}
