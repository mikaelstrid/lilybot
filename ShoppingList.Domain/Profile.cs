using Lily.Core.Domain.Model;

namespace Lily.ShoppingList.Domain
{
    public class Profile : AggregateRoot
    {
        internal Profile() : base() { }

        public Profile(string username) : base(username) { }

        public string Friends { get; set; }
    }
}
