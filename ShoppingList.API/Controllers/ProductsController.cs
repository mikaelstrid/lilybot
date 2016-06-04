using System;
using System.Threading.Tasks;
using System.Web.Http;
using Lily.Core.Application;
using Lily.ShoppingList.Domain;

namespace Lily.ShoppingList.Api.Controllers
{
    [RoutePrefix("api/products")]
    public class ProductsController : ApiController
    {
        private readonly IAggregateRepository<Product> _repository;

        public ProductsController(IAggregateRepository<Product> repository)
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
            return Ok(await _repository.GetById(id));
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Post([FromBody] CreateOrUpdateProductApiModel model)
        {
            var newProduct = new Product { Name = model.Name };
            await _repository.AddOrUpdate(newProduct);
            return Ok(newProduct);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Put(Guid id, [FromBody] CreateOrUpdateProductApiModel model)
        {
            var product = await _repository.GetById(id);
            if (product == null) return BadRequest("No product found with the specified id.");

            product.Name = model.Name;
            await _repository.AddOrUpdate(product);
            return Ok(product);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete(Guid id)
        {
            await _repository.DeleteById(id);
            return Ok();
        }
    }

    public class CreateOrUpdateProductApiModel
    {
        public string Name { get; set; }
    }
}
