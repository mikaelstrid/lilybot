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
        protected readonly DbContext Context;
        protected readonly DbSet<T> DbSet;
        
        public EntityFrameworkAggregateRepository(DbContext context)
        {
            Context = context;
            DbSet = context.Set<T>();
        }

        public virtual IEnumerable<T> GetAll(string username, string includeProperties = "")
        {
            IQueryable<T> query = DbSet;

            query = query.Where(e => e.Username == username);

            foreach (var includeProperty in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)) {
                query = query.Include(includeProperty);
            }

            return query.ToList();
        }

        public virtual IEnumerable<T> Get(
            string username, 
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            int? count = null,
            string includeProperties = "")
        {
            IQueryable<T> query = DbSet;

            query = query.Where(e => e.Username == username);

            if (predicate != null) {
                query = query.Where(predicate);
            }

            foreach (var includeProperty in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)) {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (count.HasValue)
            {
                query = query.Take(count.Value);
            }

            return query.ToList();
        }

        public virtual T GetById(string username, int id, string includeProperties = "")
        {
            return Get(username, a => a.Id == id, includeProperties: includeProperties).SingleOrDefault();
        }

        public virtual void InsertOrUpdate(string username, T aggregate)
        {
            if (aggregate.Id == 0)
            {
                DbSet.Add(aggregate);
                Context.SaveChanges();
            }
            else
            {
                var existingAggregate = Get(a => a.Id == aggregate.Id).SingleOrDefault();

                if (existingAggregate != null)
                {
                    if (existingAggregate.Username != username) throw new SecurityException("User not authorized to update entity");
                    Context.Entry(aggregate).State = EntityState.Modified;
                    Context.SaveChanges();
                }
                else
                {
                    throw new ArgumentException("No aggregate to update found.");
                }
            }
        }

        public virtual void Delete(string username, T aggregate)
        {
            DeleteById(username, aggregate.Id);
        }

        public virtual void DeleteById(string username, int id)
        {
            var existingAggregate = GetById(username, id);

            if (existingAggregate == null) return;

            if (Context.Entry(existingAggregate).State == EntityState.Detached) DbSet.Attach(existingAggregate);

            DbSet.Remove(existingAggregate);
            Context.SaveChanges();
        }



        protected IEnumerable<T> Get(Expression<Func<T, bool>> predicate, string includeProperties = "")
        {
            IQueryable<T> query = DbSet;

            if (predicate != null) {
                query = query.Where(predicate);
            }

            foreach (var includeProperty in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)) {
                query = query.Include(includeProperty);
            }

            return query.ToList();
        }
    }
}
