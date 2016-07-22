using Lilybot.Core.Domain.Model;

namespace Lilybot.Commute.Domain
{
    public class Profile : AggregateRoot
    {
        internal Profile() : base() { }

        public Profile(string username) : base(username) { }
    }
}
