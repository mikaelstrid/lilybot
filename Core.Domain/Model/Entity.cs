namespace Lilybot.Core.Domain.Model
{
    public abstract class Entity<TId>
    {
        public TId Id { get; set; }
    }
}
