using System;
using Newtonsoft.Json;

namespace Lily.Core.Domain.Model
{
    [Serializable]
    public abstract class AggregateRoot : Entity<Guid>, IAggregate
    {
        protected AggregateRoot() { }
        protected AggregateRoot(string username, Guid id) : base(id) { Username = username; }

        [JsonProperty(PropertyName = "username")]
        public string Username { get; internal set; }
    }
}
