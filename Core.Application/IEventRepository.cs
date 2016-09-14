using System;
using System.Collections.Generic;
using Lilybot.Core.Domain.Model;

namespace Lilybot.Core.Application
{
    public interface IEventRepository
    {
        IEnumerable<Event> GetAll(string username, string includeProperties = "");
        IEnumerable<Event> GetFrom(string username, DateTime? timestampUtc, string includeProperties = "");
        void Insert(string username, Event @event);
    }
}