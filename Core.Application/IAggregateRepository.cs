using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Lily.Core.Domain.Model;

namespace Lily.Core.Application
{
    public interface IAggregateRepository<T> where T: class, IAggregate
    {
        IEnumerable<T> GetAll(string username);
        IEnumerable<T> Get(string username, Expression<Func<T, bool>> predicate);
        T GetById(string username, int id);
        void InsertOrUpdate(string username, T aggregate);
        void Delete(string username, T aggregate);
        void DeleteById(string username, int id);
    }
}