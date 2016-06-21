//using System;
//using System.Linq;
//using System.Web.Http;
//using Lily.Core.Application;
//using Lily.ShoppingList.Domain;

//namespace Lily.ShoppingList.Api.Controllers
//{
//    [Authorize]
//    //[CheckIfFriendActionFilter] Added in the DI/Autofac configuration
//    [RoutePrefix("api/stores")]
//    public class StoresController : FriendsApiControllerBase
//    {
//        private readonly IAggregateRepository<Store> _repository;

//        public StoresController(IAggregateRepository<Store> repository)
//        {
//            _repository = repository;
//        }

//        [HttpGet]
//        [Route("")]
//        public IHttpActionResult Get()
//        {
//            return Ok(_repository.GetAll(Username));
//        }

//        [HttpGet]
//        [Route("{id}")]
//        public IHttpActionResult Get(Guid id)
//        {
//            var store = _repository.GetById(Username, id);
//            if (store == null) return NotFound();
//            return Ok(store);
//        }

//        [HttpPost]
//        [Route("")]
//        public IHttpActionResult Post([FromBody] CreateOrUpdateStoreApiModel model)
//        {
//            var newStore = new Store(Username) { Name = model.Name };
//            _repository.AddOrUpdate(Username, newStore);
//            return Ok(newStore);
//        }

//        [HttpPut]
//        [Route("{id}")]
//        public IHttpActionResult Put(Guid id, [FromBody] CreateOrUpdateProductApiModel model)
//        {
//            var store = _repository.GetById(Username, id);
//            if (store == null) return BadRequest("No store found with the specified id.");

//            store.Name = model.Name;
//            _repository.AddOrUpdate(Username, store);
//            return Ok(store);
//        }

//        [HttpDelete]
//        [Route("{id}")]
//        public IHttpActionResult Delete(Guid id)
//        {
//            _repository.DeleteById(Username, id);
//            return Ok();
//        }


//        // === SECTIONS ===

//        [HttpPost]
//        [Route("{id}/sections")]
//        public IHttpActionResult PostSection(Guid id, [FromBody] CreateOrUpdateStoreSectionApiModel model)
//        {
//            var store = _repository.GetById(Username, id);
//            if (store == null) return BadRequest("No store found with the specified id.");

//            var newStoreSection = new StoreSection { Name = model.Name };
//            store.Sections.Add(newStoreSection);
            
//            _repository.AddOrUpdate(Username, store);
//            return Ok(newStoreSection);
//        }

//        [HttpPut]
//        [Route("{storeId}/sections/{sectionId}")]
//        public IHttpActionResult PutSection(Guid storeId, Guid sectionId, [FromBody] CreateOrUpdateStoreSectionApiModel model)
//        {
//            var store = _repository.GetById(Username, storeId);
//            if (store == null) return BadRequest("No store found with the specified id.");

//            var section = store.Sections.FirstOrDefault(s => s.Id == sectionId);
//            if (section == null) return BadRequest("No section found with the specified id.");

//            section.Name = model.Name;
//            _repository.AddOrUpdate(Username, store);

//            return Ok(section);
//        }

//        [HttpDelete]
//        [Route("{storeId}/sections/{sectionId}")]
//        public IHttpActionResult DeleteSection(Guid storeId, Guid sectionId)
//        {
//            var store = _repository.GetById(Username, storeId);
//            if (store == null) return BadRequest("No store found with the specified id.");

//            var section = store.Sections.FirstOrDefault(s => s.Id == sectionId);
//            if (section == null) return BadRequest("No section found with the specified id.");
//            store.Sections.Remove(section);

//            _repository.AddOrUpdate(Username, store);
//            return Ok();
//        }

//        [HttpPut]
//        [Route("{storeId}/sections/{sectionId}/movesectionup")]
//        public IHttpActionResult MoveSectionUp(Guid storeId, Guid sectionId)
//        {
//            var store = _repository.GetById(Username, storeId);
//            if (store == null) return BadRequest("No store found with the specified id.");

//            var section = store.Sections.FirstOrDefault(s => s.Id == sectionId);
//            if (section == null) return BadRequest("No section found with the specified id.");

//            var oldIndex = store.Sections.IndexOf(section);
//            if (oldIndex == 0) return Ok();

//            store.Sections.Remove(section);
//            store.Sections.Insert(oldIndex-1, section);

//            _repository.AddOrUpdate(Username, store);
//            return Ok();
//        }

//        [HttpPut]
//        [Route("{storeId}/sections/{sectionId}/movesectiondown")]
//        public IHttpActionResult MoveSectionDown(Guid storeId, Guid sectionId)
//        {
//            var store = _repository.GetById(Username, storeId);
//            if (store == null) return BadRequest("No store found with the specified id.");

//            var section = store.Sections.FirstOrDefault(s => s.Id == sectionId);
//            if (section == null) return BadRequest("No section found with the specified id.");

//            var oldIndex = store.Sections.IndexOf(section);
//            if (oldIndex == store.Sections.Count - 1) return Ok();

//            store.Sections.Remove(section);
//            store.Sections.Insert(oldIndex + 1, section);

//            _repository.AddOrUpdate(Username, store);
//            return Ok();
//        }


//        [HttpPut]
//        [Route("{storeId}/sections/{sectionId}/moveproducttosection/{productId}")]
//        public IHttpActionResult MoveProductToSection(Guid storeId, Guid productId, Guid sectionId)
//        {
//            var store = _repository.GetById(Username, storeId);
//            if (store == null) return BadRequest("No store found with the specified id.");

//            // Try to find the section containing the product
//            var currentSection = store.Sections.FirstOrDefault(s => s.ProductIds.Contains(productId));

//            // If no section was found, check the ignore list
//            if (currentSection == null && store.IgnoredProducts.ProductIds.Contains(productId)) currentSection = store.IgnoredProducts;

//            // Get the section to move the product to
//            var newSection = store.Sections.FirstOrDefault(s => s.Id == sectionId);
//            if (newSection == null && store.IgnoredProducts.Id == sectionId) newSection = store.IgnoredProducts; 
//            if (newSection == null) return BadRequest("No section found to move the produt to.");

//            // Move the 
//            if (currentSection != null) currentSection.ProductIds.Remove(productId);
//            newSection.ProductIds.Add(productId);
//            _repository.AddOrUpdate(Username, store);

//            return Ok();
//        }
//    }

//    public class CreateOrUpdateStoreApiModel
//    {
//        public string Name { get; set; }
//    }

//    public class CreateOrUpdateStoreSectionApiModel
//    {
//        public string Name { get; set; }
//    }
//}
