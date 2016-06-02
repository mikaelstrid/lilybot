using System;
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

        //[HttpGet]
        //public async Task<IHttpActionResult> Get(Guid id)
        //{
        //    return Ok(await _repository.GetById(id));
        //}

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Post([FromBody] CreateOrUpdateStoreApiModel model)
        {
            var newStore = new Store { Name = model.Name };
            await _repository.AddOrUpdate(newStore);
            return Ok(newStore);
        }

        //[HttpPut]
        //public async Task<IHttpActionResult> Put(Guid id, [FromBody] CreateOrUpdateProductApiModel model)
        //{
        //    var updatedProduct = new Product(id) { Name = model.Name };
        //    await _repository.AddOrUpdate(updatedProduct);
        //    return Ok(updatedProduct);
        //}

        //[HttpDelete]
        //public IHttpActionResult Delete(Guid id)
        //{
        //    _repository.DeleteById(id);
        //    return Ok();
        //}
    }

    public class CreateOrUpdateStoreApiModel
    {
        public string Name { get; set; }
    }
}
