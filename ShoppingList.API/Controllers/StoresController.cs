using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Lily.Core.Application;
using Lily.ShoppingList.Domain;

namespace Lily.ShoppingList.Api.Controllers
{
    [RoutePrefix("api/stores")]
    public class StoresController : ApiController
    {
        private readonly IAggregateRepository<Store> _repository;

        public StoresController(IAggregateRepository<Store> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> Get()
        {
            return Ok(await _repository.GetAll());
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IHttpActionResult> Get(Guid id)
        {
            //if (!id.HasValue) return BadRequest("No guid parameters specified.");

            //return Ok(await _repository.GetById(id.Value));
            return Ok(await _repository.GetById(id));
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Post([FromBody] CreateOrUpdateStoreApiModel model)
        {
            var newStore = new Store { Name = model.Name };
            await _repository.AddOrUpdate(newStore);
            return Ok(newStore);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Put(Guid id, [FromBody] CreateOrUpdateProductApiModel model)
        {
            var updatedStore = new Store(id) { Name = model.Name };
            await _repository.AddOrUpdate(updatedStore);
            return Ok(updatedStore);
        }

        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult Delete(Guid id)
        {
            _repository.DeleteById(id);
            return Ok();
        }


        // === SECTIONS ===

        [HttpPost]
        [Route("{id}/sections")]
        public async Task<IHttpActionResult> PostSection(Guid id, [FromBody] CreateOrUpdateStoreSectionApiModel model)
        {
            var store = await _repository.GetById(id);
            if (store == null) return BadRequest("No store found with the specified id.");

            var newStoreSection = new StoreSection { Name = model.Name };
            store.Sections.Add(newStoreSection);
            
            await _repository.AddOrUpdate(store);
            return Ok(newStoreSection);
        }

        [HttpDelete]
        [Route("{storeId}/sections/{sectionId}")]
        public async Task<IHttpActionResult> DeleteSection(Guid storeId, Guid sectionId)
        {
            var store = await _repository.GetById(storeId);
            if (store == null) return BadRequest("No store found with the specified id.");

            var section = store.Sections.FirstOrDefault(s => s.Id == sectionId);
            if (section == null) return BadRequest("No section found with the specified id.");
            store.Sections.Remove(section);

            await _repository.AddOrUpdate(store);
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
