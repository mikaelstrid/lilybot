namespace Lilybot.Core.Domain.Model
{
    public abstract class AggregateRoot : Entity<int>, IAggregate
    {
        protected AggregateRoot() { }

        protected AggregateRoot(string username) { Username = username; }

        public string Username { get; set; }
    }
}
