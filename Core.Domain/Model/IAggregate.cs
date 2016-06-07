using System;

namespace Lily.Core.Domain.Model
{
    public interface IAggregate
    {
        Guid Id { get; }
        string Type { get; }
        string Username { get; }
    }
}