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
        private readonly IAggregateRepository<Product> _productsRepository;

        public ProductsController(IAggregateRepository<Product> productsRepository)
        {
            _productsRepository = productsRepository;
        }

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> Get()
        {
            return Ok(await _productsRepository.GetAll());
        }

        [HttpGet]
        public async Task<IHttpActionResult> Get(Guid id)
        {
            return Ok(await _productsRepository.GetById(id));
        }

        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] CreateOrUpdateProductApiModel model)
        {
            try
            {
                var newProduct = new Product { Name = model.Name };
                await _productsRepository.AddOrUpdate(newProduct);
                return Ok(newProduct);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        [HttpPut]
        public async Task<IHttpActionResult> Put(Guid id, [FromBody] CreateOrUpdateProductApiModel model)
        {
            var updatedProduct = new Product(id) { Name = model.Name };
            await _productsRepository.AddOrUpdate(updatedProduct);
            return Ok(updatedProduct);
        }

        [HttpDelete]
        public IHttpActionResult Delete(Guid id)
        {
            _productsRepository.DeleteById(id);
            return Ok();
        }
    }

    public class CreateOrUpdateProductApiModel
    {
        public string Name { get; set; }
    }
}
