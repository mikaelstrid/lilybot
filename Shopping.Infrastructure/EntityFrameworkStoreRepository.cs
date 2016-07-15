using System;
using System.Data.Entity;
using System.Linq;
using System.Security;
using Lilybot.Core.Infrastructure.Persistence.EntityFramework;
using Lilybot.Shopping.Application;
using Lilybot.Shopping.Domain;

// ReSharper disable SimplifyLinqExpression

namespace Lilybot.Shopping.Infrastructure
{
    public class EntityFrameworkStoreRepository : EntityFrameworkAggregateRepository<Store>, IStoreRepository
    {
        public EntityFrameworkStoreRepository(DbContext context) : base(context) { }


        public override void InsertOrUpdate(string username, Store aggregate)
        {
            if (aggregate.Id == 0)
            {
                base.InsertOrUpdate(username, aggregate);
            }
            else
            {
                var existingAggregate = Get(a => a.Id == aggregate.Id).SingleOrDefault();

                if (existingAggregate != null)
                {
                    if (existingAggregate.Username != username) throw new SecurityException("User not authorized to update entity");
                    Context.Entry(aggregate).State = EntityState.Modified;

                    // Check if any section in the existing store (from the db) has been removed in the 
                    // section that should be sent to the database
                    foreach (var existingSection in existingAggregate.Sections)
                    {
                        var matchingSection = aggregate.Sections.SingleOrDefault(s => s.Id == existingSection.Id);
                        if (matchingSection == null)
                        {
                            Context.Entry(existingSection).State = EntityState.Deleted;
                        }
                        else
                        {
                            // Make the same check for associated products
                            foreach (var existingProduct in existingSection.Products)
                            {
                                if (!matchingSection.Products.Any(p => p.Id == existingProduct.Id))
                                {
                                    Context.Entry(existingProduct).State = EntityState.Deleted;
                                }
                            }
                        }
                    }

                    Context.SaveChanges();
                }
                else
                {
                    throw new ArgumentException("No aggregate to update found.");
                }
            }
        }


        public void DeleteSectionById(string username, int storeId, int sectionId)
        {
            var store = GetById(username, storeId, "Sections");
            Context.Entry(store.Sections.Single(s => s.Id == sectionId)).State = EntityState.Deleted;
            Context.SaveChanges();
        }

        //public void RemoveProductFromSection(string username, int storeId, int sectionId)
        //{
        //    var store = GetById(username, storeId, "Sections");
        //    Context.Entry(store.Sections.Single(s => s.Id == sectionId)).State = EntityState.Deleted;
        //    Context.SaveChanges();
        //}

    }
}
