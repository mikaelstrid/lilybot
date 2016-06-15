using System;
using System.Threading.Tasks;
using System.Web.Http;
using Lily.Core.Application;
using Lily.ShoppingList.Domain;

namespace Lily.ShoppingList.Api.Controllers
{
    [Authorize]
    //[CheckIfFriendActionFilter] Added in the DI/Autofac configuration
    [RoutePrefix("api/products")]
    public class ProductsController : FriendsApiControllerBase
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
            return Ok(await _repository.GetAll(Username));
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IHttpActionResult> Get(Guid id)
        {
            return Ok(await _repository.GetById(Username, id));
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Post([FromBody] CreateOrUpdateProductApiModel model)
        {
            var newProduct = new Product(Username) { Name = model.Name };
            await _repository.AddOrUpdate(Username, newProduct);
            return Ok(newProduct);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Put(Guid id, [FromBody] CreateOrUpdateProductApiModel model)
        {
            var product = await _repository.GetById(Username, id);
            if (product == null) return BadRequest("No product found with the specified id.");

            product.Name = model.Name;
            await _repository.AddOrUpdate(Username, product);
            return Ok(product);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete(Guid id)
        {
            await _repository.DeleteById(Username, id);
            return Ok();
        }


    }

    public class CreateOrUpdateProductApiModel
    {
        public string Name { get; set; }
    }
}
