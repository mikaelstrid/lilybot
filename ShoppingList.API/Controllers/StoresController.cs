using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using AutoMapper;
using Lily.Core.Application;
using Lily.ShoppingList.Application;
using Lily.ShoppingList.Domain;

namespace Lily.ShoppingList.Api.Controllers
{
    [Authorize]
    //[CheckIfFriendActionFilter] Added in the DI/Autofac configuration
    [RoutePrefix("api/stores")]
    public class StoresController : FriendsApiControllerBase
    {
        private readonly IStoreRepository _storeRepository;
        private readonly IAggregateRepository<Product> _productRepository;

        public StoresController(IStoreRepository storeRepository, IAggregateRepository<Product> productRepository )
        {
            _storeRepository = storeRepository;
            _productRepository = productRepository;
        }



        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            var allStores = _storeRepository.GetAll(Username);
            var allStoreDtos = DefaultMapper.Map<IEnumerable<Store>, IEnumerable<StoreDto>>(allStores);
            return Ok(allStoreDtos);
        }

        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult Get(int id)
        {
            var store = _storeRepository.GetById(Username, id, "Sections.Products");
            if (store == null) return NotFound();
            store.Sections = store.Sections.OrderBy(s => s.Order).ToList();
            var storeDto = DefaultMapper.Map<StoreDto>(store);
            return Ok(storeDto);
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Post([FromBody] CreateOrUpdateStoreApiModel model)
        {
            var newStore = new Store(Username)
            {
                Name = model.Name
            };
            _storeRepository.InsertOrUpdate(Username, newStore);
            return Ok(DefaultMapper.Map<StoreDto>(newStore));
        }
        
        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult Put(int id, [FromBody] CreateOrUpdateProductApiModel model)
        {
            var store = _storeRepository.GetById(Username, id);
            if (store == null) return BadRequest("No store found with the specified id.");

            store.Name = model.Name;
            _storeRepository.InsertOrUpdate(Username, store);
            return Ok(DefaultMapper.Map<StoreDto>(store));
        }

        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult Delete(int id)
        {
            _storeRepository.DeleteById(Username, id);
            return Ok();
        }


        // === SECTIONS ===

        [HttpPost]
        [Route("{id}/sections")]
        public IHttpActionResult PostSection(int id, [FromBody] CreateOrUpdateStoreSectionApiModel model)
        {
            var store = _storeRepository.GetById(Username, id);
            if (store == null) return BadRequest("No store found with the specified id.");

            var newStoreSection = new StoreSection { Name = model.Name, Order = store.Sections.Any() ? store.Sections.Max(s => s.Order) + 1 : 1 };
            store.Sections.Add(newStoreSection);

            _storeRepository.InsertOrUpdate(Username, store);
            return Ok(DefaultMapper.Map<StoreSectionDto>(newStoreSection));
        }

        [HttpPut]
        [Route("{storeId}/sections/{sectionId}")]
        public IHttpActionResult PutSection(int storeId, int sectionId, [FromBody] CreateOrUpdateStoreSectionApiModel model)
        {
            var store = _storeRepository.GetById(Username, storeId);
            if (store == null) return BadRequest("No store found with the specified id.");

            var section = store.Sections.FirstOrDefault(s => s.Id == sectionId);
            if (section == null) return BadRequest("No section found with the specified id.");

            section.Name = model.Name;
            _storeRepository.InsertOrUpdate(Username, store);

            return Ok(DefaultMapper.Map<StoreSectionDto>(section));
        }

        [HttpDelete]
        [Route("{storeId}/sections/{sectionId}")]
        public IHttpActionResult DeleteSection(int storeId, int sectionId)
        {
            _storeRepository.DeleteSectionById(Username, storeId, sectionId);
            return Ok();
        }

        [HttpPut]
        [Route("{storeId}/sections/{sectionId}/movesectionup")]
        public IHttpActionResult MoveSectionUp(int storeId, int sectionId)
        {
            var store = _storeRepository.GetById(Username, storeId);
            if (store == null) return BadRequest("No store found with the specified id.");

            var section = store.Sections.FirstOrDefault(s => s.Id == sectionId);
            if (section == null) return BadRequest("No section found with the specified id.");

            var orderedSections = store.Sections.OrderBy(s => s.Order).ToList();

            if (section == orderedSections.First()) return Ok();

            var currentIndex = orderedSections.IndexOf(section);
            var sectionBefore = orderedSections[currentIndex - 1];

            var currentSectionOrder = section.Order;
            var sectionBeforeOrder = sectionBefore.Order;

            section.Order = sectionBeforeOrder;
            sectionBefore.Order = currentSectionOrder;

            _storeRepository.InsertOrUpdate(Username, store);
            return Ok();
        }

        [HttpPut]
        [Route("{storeId}/sections/{sectionId}/movesectiondown")]
        public IHttpActionResult MoveSectionDown(int storeId, int sectionId)
        {
            var store = _storeRepository.GetById(Username, storeId);
            if (store == null) return BadRequest("No store found with the specified id.");

            var section = store.Sections.FirstOrDefault(s => s.Id == sectionId);
            if (section == null) return BadRequest("No section found with the specified id.");

            var orderedSections = store.Sections.OrderBy(s => s.Order).ToList();

            if (section == orderedSections.Last()) return Ok();

            var currentIndex = orderedSections.IndexOf(section);
            var sectionAfter = orderedSections[currentIndex + 1];

            var currentSectionOrder = section.Order;
            var sectionAfterOrder = sectionAfter.Order;

            section.Order = sectionAfterOrder;
            sectionAfter.Order = currentSectionOrder;

            _storeRepository.InsertOrUpdate(Username, store);
            return Ok();
        }


        [HttpPut]
        [Route("{storeId}/sections/{sectionId}/moveproducttosection/{productId}")]
        public IHttpActionResult MoveProductToSection(int storeId, int productId, int sectionId)
        {
            var store = _storeRepository.GetById(Username, storeId, "Sections.Products");
            if (store == null) return BadRequest("No store found with the specified id.");

            var product = _productRepository.GetById(Username, productId);
            if (product == null) return BadRequest("No product found with the specified id.");

            var currentSection = store.Sections.SingleOrDefault(s => s.Products.Any(p => p.Id == productId));
            if (currentSection != null)
                currentSection.Products.Remove(product);
            else if (store.IgnoredProducts.Contains(product))
                store.IgnoredProducts.Remove(product);

            var newSection = store.Sections.SingleOrDefault(s => s.Id == sectionId);
            if (newSection != null)
                newSection.Products.Add(product);
            else
                if (sectionId == -1)
                    store.IgnoredProducts.Add(product);
                else
                    return BadRequest("No section found to move the produt to.");

            _storeRepository.InsertOrUpdate(Username, store);
            return Ok();
        }
    }


    public class CreateOrUpdateStoreApiModel
    {
        public string Name { get; set; }
    }

    public class CreateOrUpdateStoreSectionApiModel
    {
        public string Name { get; set; }
    }
}
