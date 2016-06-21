using System;
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
        public IHttpActionResult Get()
        {
            return Ok(_repository.GetAll(Username));
        }

        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult Get(int id)
        {
            return Ok(_repository.GetById(Username, id));
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Post([FromBody] CreateOrUpdateProductApiModel model)
        {
            var newProduct = new Product(Username) { Name = model.Name };
            _repository.InsertOrUpdate(Username, newProduct);
            return Ok(newProduct);
        }

        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult Put(int id, [FromBody] CreateOrUpdateProductApiModel model)
        {
            var product = _repository.GetById(Username, id);
            if (product == null) return BadRequest("No product found with the specified id.");

            product.Name = model.Name;
            _repository.InsertOrUpdate(Username, product);
            return Ok(product);
        }

        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult Delete(int id)
        {
            _repository.DeleteById(Username, id);
            return Ok();
        }
    }

    public class CreateOrUpdateProductApiModel
    {
        public string Name { get; set; }
    }
}
