using Lilybot.Core.Domain.Model;

namespace Lilybot.Shopping.Domain
{
    public class Profile : AggregateRoot
    {
        internal Profile() : base() { }

        public Profile(string username) : base(username) { }

        public string Friends { get; set; }
    }
}
