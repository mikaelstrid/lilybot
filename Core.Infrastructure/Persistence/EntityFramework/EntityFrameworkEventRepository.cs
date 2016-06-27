using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Lily.Core.Application;
using Lily.Core.Domain.Model;

namespace Lily.Core.Infrastructure.Persistence.EntityFramework
{
    public class EntityFrameworkEventRepository : IEventRepository
    {
        protected readonly DbContext Context;
        protected readonly DbSet<Event> DbSet;
        
        public EntityFrameworkEventRepository(DbContext context)
        {
            Context = context;
            DbSet = context.Set<Event>();
        }

        public virtual IEnumerable<Event> GetAll(string username, string includeProperties = "")
        {
            IQueryable<Event> query = DbSet;

            query = query.Where(e => e.Username == username);

            foreach (var includeProperty in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)) {
                query = query.Include(includeProperty);
            }

            var events = query.ToList();
            return events.Select(e => e.ToDerivedType());
        }

        public virtual void Insert(string username, Event @event)
        {
            if (@event.Id != 0) throw new ArgumentException("Can't insert existing event (Id != 0).");

            var baseEvent = @event.ToBaseType();
            DbSet.Add(baseEvent);
            Context.SaveChanges();

            @event.Id = baseEvent.Id;
        }
    }
}
