using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lily.Core.Domain.Model;

namespace Lily.Core.Application
{
    public interface IAggregateRepository<T> where T: class, IAggregate
    {
        Task<IEnumerable<T>> GetAll(string username);
        Task<IEnumerable<T>> Get(string username, Func<T, bool> predicate);
        Task<T> GetById(string usernamem, Guid id);
        Task AddOrUpdate(string username, T aggregate);
        Task Delete(string username, T aggregate);
        Task DeleteById(string username, Guid id);
    }
}