namespace Lilybot.Core.Domain.Model
{
    public interface IAggregate
    {
        int Id { get; }
        string Username { get; }
    }
}