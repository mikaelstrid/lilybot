using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Http;
using Lily.Core.Application;
using Lily.ShoppingList.Application;
using Lily.ShoppingList.Domain;
using Lily.ShoppingList.Domain.Events;

namespace Lily.ShoppingList.Api.Controllers
{
    [RoutePrefix("api/migrate")]
    public class MigrateController : ApiController
    {
        private const string MIGRATION_USERNAME = "10153830410348370";

        private readonly IStoreRepository _storesRepository;
        private readonly IAggregateRepository<Product> _productsRepository;
        private readonly IEventRepository _eventRepository;

        public MigrateController(
            IStoreRepository storesRepository,
            IAggregateRepository<Product> productsRepository,
            IEventRepository eventRepository)
        {
            _storesRepository = storesRepository;
            _productsRepository = productsRepository;
            _eventRepository = eventRepository;
        }

        [HttpGet]
        [Route("{id?}")]
        public IHttpActionResult Get(int? id = null)
        {
            var migrationUsername = id?.ToString() ?? MIGRATION_USERNAME;

            var productParts = GetParts("Produkter.csv");
            var products = productParts.Skip(1).Select(p =>
            {
                var newProduct = new Product(migrationUsername)
                {
                    Name = p[1],
                    Count = int.Parse(p[2]),
                    CountUpdateTimestampUtc = DateTime.UtcNow
                };
                _productsRepository.InsertOrUpdate(migrationUsername, newProduct);
                return new
                {
                    OldId = Convert.ToInt32(p[0]),
                    NewProduct = newProduct
                };
            }).ToList();

            foreach (var product in products)
            {
                _eventRepository.Insert(migrationUsername, 
                    new ProductMigratedEvent(
                        migrationUsername, 
                        product.NewProduct.Id,
                        product.NewProduct.Count,
                        product.NewProduct.CountUpdateTimestampUtc));
            }

            var storeSectionProductJoins = GetParts("StoreSectionProducts.csv")
                .Skip(1)
                .Select(p => new
                {
                    OldStoreSectionId = Convert.ToInt32(p[0]),
                    OldProductId = Convert.ToInt32(p[1])
                });

            var storeSectionParts = GetParts("StoreSections.csv");
            var storesSections = storeSectionParts
                .Skip(1)
                .Select(p => new
                {
                    OldId = Convert.ToInt32(p[0]),
                    Name = p[1],
                    OldStoreId = Convert.ToInt32(p[2])
                }).ToList();

            var storeParts = new List<string[]> { new[] { "3", "Kvantum" } };
            var stores = storeParts.Select(p =>
            {
                var oldStoreId = Convert.ToInt32(p[0]);
                var sectionCount = 1;
                var newStore = new Store(migrationUsername)
                {
                    Name = p[1],
                    Sections = storesSections
                        .Where(s => s.OldStoreId == oldStoreId)
                        .Select(s =>
                        {
                            var section = new StoreSection
                            {
                                Name = s.Name,
                                Products = new List<Product>(),
                                Order = sectionCount++
                            };

                            foreach (var join in storeSectionProductJoins.Where(j => j.OldStoreSectionId == s.OldId))
                            {
                                var product = products.FirstOrDefault(pr => pr.OldId == join.OldProductId);
                                if (product != null) section.Products.Add(product.NewProduct);
                            }

                            return section;
                        }).ToList()
                };
                _storesRepository.InsertOrUpdate(migrationUsername, newStore);
                return new { OldId = oldStoreId, NewStore = newStore };
            }).ToList();

            return Ok();
        }

        private static List<string[]> GetParts(string filename)
        {
            var path = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/" + filename);
            var parts = File.ReadAllLines(path, Encoding.UTF8).Select(l => l.Split(';'));
            return parts.ToList();
        }
    }

}
