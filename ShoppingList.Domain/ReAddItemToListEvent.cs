using System;
using Lily.Core.Domain.Model;

namespace Lily.ShoppingList.Domain
{
    public class ReAddItemToListEvent : Event, IImuttable
    {
        public ReAddItemToListEvent(string username) : base(username) { }

        public Guid OldItemId { get; set; }
    }
}
