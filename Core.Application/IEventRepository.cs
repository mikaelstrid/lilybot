using System.Collections.Generic;
using Lily.Core.Domain.Model;

namespace Lily.Core.Application
{
    public interface IEventRepository
    {
        IEnumerable<Event> GetAll(string username, string includeProperties = "");
        void Insert(string username, Event @event);
    }
}