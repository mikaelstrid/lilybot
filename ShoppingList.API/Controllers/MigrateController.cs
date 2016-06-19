using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Lily.Core.Application;
using Lily.Core.Domain.Model;
using Lily.ShoppingList.Domain;

namespace Lily.ShoppingList.Api.Controllers
{
    [RoutePrefix("api/migrate")]
    public class MigrateController : ApiController
    {
        private const string MIGRATION_USERNAME = "10153830410348370";

        private readonly IAggregateRepository<Store> _storesRepository;
        private readonly IAggregateRepository<Product> _productsRepository;
        private readonly IAggregateRepository<AddItemToListEvent> _addItemToListEventRepository;
        private readonly IAggregateRepository<ReAddItemToListEvent> _reAddItemToListEventRepository;
        private readonly IAggregateRepository<RemoveItemFromListEvent> _removeItemToListEventRepository;
        private readonly IAggregateRepository<MarkItemAsDoneEvent> _markItemAsDoneEventRepository;
        private readonly IAggregateRepository<SetCommentEvent> _setCommentEventRepository;

        public MigrateController(
            IAggregateRepository<Store> storesRepository,
            IAggregateRepository<Product> productsRepository,
            IAggregateRepository<AddItemToListEvent> addItemToListEventRepository,
            IAggregateRepository<ReAddItemToListEvent> reAddItemToListEventRepository,
            IAggregateRepository<RemoveItemFromListEvent> removeItemToListEventRepository,
            IAggregateRepository<MarkItemAsDoneEvent> markItemAsDoneEventRepository,
            IAggregateRepository<SetCommentEvent> setCommentEventRepository)
        {
            _storesRepository = storesRepository;
            _productsRepository = productsRepository;
            _addItemToListEventRepository = addItemToListEventRepository;
            _reAddItemToListEventRepository = reAddItemToListEventRepository;
            _removeItemToListEventRepository = removeItemToListEventRepository;
            _markItemAsDoneEventRepository = markItemAsDoneEventRepository;
            _setCommentEventRepository = setCommentEventRepository;
        }

        [HttpGet]
        [Route("{id?}")]
        public async Task<IHttpActionResult> Get(int? id = null)
        {
            var migrationUsername = id?.ToString() ?? MIGRATION_USERNAME;

            var productParts = GetParts("Produkter.csv");
            var products = await Task.WhenAll(
                productParts.Skip(1).Select(async p =>
                {
                    var newProduct = new Product(migrationUsername) {Name = p[1]};
                    await _productsRepository.AddOrUpdate(migrationUsername, newProduct);
                    return
                        new {OldId = Convert.ToInt32(p[0]), NewProduct = newProduct, OldCount = Convert.ToInt32(p[2])};
                }));

            var storeSectionProductJoins = GetParts("StoreSectionProducts.csv")
                .Skip(1)
                .Select(p => new { OldStoreSectionId = Convert.ToInt32(p[0]), OldProductId = Convert.ToInt32(p[1]) });

            var storeSectionParts = GetParts("StoreSections.csv");
            var storesSections = storeSectionParts.Skip(1).Select(p => new { OldId = Convert.ToInt32(p[0]), Name = p[1], OldStoreId = Convert.ToInt32(p[2]) }).ToList();

            var storeParts = new List<string[]> { new[] { "3", "Kvantum" } };
            var stores = storeParts.Select(async p =>
            {
                var oldStoreId = Convert.ToInt32(p[0]);
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
                                ProductIds = new List<Guid>()
                            };

                            foreach (var join in storeSectionProductJoins.Where(j => j.OldStoreSectionId == s.OldId))
                            {
                                var product = products.FirstOrDefault(pr => pr.OldId == join.OldProductId);
                                if (product != null) section.ProductIds.Add(product.NewProduct.Id);
                            }

                            return section;
                        }).ToList()
                };
                await _storesRepository.AddOrUpdate(migrationUsername, newStore);
                return new { OldId = oldStoreId, NewStore = newStore };
            }).ToList();

            await Task.WhenAll(stores);

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
