using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lily.Core.Domain.Model;

namespace Lily.Core.Application
{
    public interface IAggregateRepository<T> where T: class, IAggregate
    {
        Task<IEnumerable<T>> GetAll();
        Task<IEnumerable<T>> Get(Func<T, bool> predicate);
        Task<T> GetById(Guid id);
        Task AddOrUpdate(T aggregate);
        Task Delete(T aggregate);
        Task DeleteById(Guid id);
    }
}