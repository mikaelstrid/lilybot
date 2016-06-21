using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Security;
using Lily.Core.Application;
using Lily.Core.Domain.Model;

namespace Lily.Core.Infrastructure.Persistence.EntityFramework
{
    public class EntityFrameworkAggregateRepository<T> : IAggregateRepository<T> where T : class, IAggregate
    {
        private readonly DbContext _context;
        private readonly DbSet<T> _dbSet;
        
        public EntityFrameworkAggregateRepository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public IEnumerable<T> GetAll(string username)
        {
            return _dbSet.Where(e => e.Username == username);
        }

        public IEnumerable<T> Get(string username, Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(e => e.Username == username).Where(predicate);
        }

        public T GetById(string username, int id)
        {
            return Get(username, a => a.Id == id).SingleOrDefault();
        }

        public void InsertOrUpdate(string username, T aggregate)
        {
            if (aggregate.Id == 0)
            {
                _dbSet.Add(aggregate);
                _context.SaveChanges();
            }
            else
            {
                var existingAggregate = Get(a => a.Id == aggregate.Id).SingleOrDefault();

                if (existingAggregate != null)
                {
                    if (existingAggregate.Username != username) throw new SecurityException("User not authorized to update entity");
                    _context.Entry(aggregate).State = EntityState.Modified;
                    _context.SaveChanges();
                }
                else
                {
                    throw new ArgumentException("No aggregate to update found.");
                }
            }
        }

        public void Delete(string username, T aggregate)
        {
            DeleteById(username, aggregate.Id);
        }

        public void DeleteById(string username, int id)
        {
            var existingAggregate = GetById(username, id);

            if (existingAggregate == null) return;

            if (_context.Entry(existingAggregate).State == EntityState.Detached) _dbSet.Attach(existingAggregate);

            _dbSet.Remove(existingAggregate);
            _context.SaveChanges();
        }



        protected IEnumerable<T> Get(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }
    }
}
