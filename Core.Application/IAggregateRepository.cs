using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Lilybot.Core.Domain.Model;

namespace Lilybot.Core.Application
{
    public interface IAggregateRepository<T> where T: class, IAggregate
    {
        IEnumerable<T> GetAll(string username = "", string includeProperties = "");
        IEnumerable<T> Get(
            string username, 
            Expression<Func<T, bool>> predicate = null, 
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, 
            int? count = null, 
            string includeProperties = "");
        T GetById(string username, int id, string includeProperties = "");
        void InsertOrUpdate(string username, T aggregate);
        void Update(T aggregate);
        void Delete(string username, T aggregate);
        void DeleteById(string username, int id);
    }
}