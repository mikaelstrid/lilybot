using System;
using Newtonsoft.Json;

namespace Lily.Core.Domain.Model
{
    [Serializable]
    public abstract class Entity<TId>
    {
        protected Entity() {}
        protected Entity(TId id) { Id = id; }

        [JsonProperty(PropertyName = "id")]
        public TId Id { get; internal set; } // internal set is required for deserializing

        [JsonProperty(PropertyName = "type")]
        public virtual string Type => GetType().Name;


        public override bool Equals(object obj)
        {
            var other = obj as Entity<TId>;
            return !ReferenceEquals(other, null) && Id.Equals(other.Id);
        }

        public override int GetHashCode() => Id.GetHashCode();
    }
}
