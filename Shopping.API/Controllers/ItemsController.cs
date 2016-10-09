using System.Linq;
using System.Web.Http;
using Lilybot.Core.Application;
using Lilybot.Shopping.Application;
using Lilybot.Shopping.Domain;
using Lilybot.Shopping.Domain.Events;

namespace Lilybot.Shopping.API.Controllers
{
    [Authorize]
    //[CheckIfFriendActionFilter] Added in the DI/Autofac configuration
    [RoutePrefix("api/items")]
    public class ItemsController : FriendsApiControllerBase
    {
        private readonly IEventRepository _eventRepository;
        private readonly IItemsService _itemsService;
        private readonly IAggregateRepository<Product> _productsRepository;

        public ItemsController(IEventRepository eventRepository, IItemsService itemsService, IAggregateRepository<Product> productsRepository)
        {
            _eventRepository = eventRepository;
            _itemsService = itemsService;
            _productsRepository = productsRepository;
        }

        [HttpGet]
        [Route("active")]
        public IHttpActionResult GetActive()
        {
            return Ok(_itemsService.GetItems(Username).Where(i => i.Active));
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult PostAdd([FromBody] AddItemApiModel model)
        {
            var newEvent = new ItemAddedToListEvent(Username, model.ProductId);
            _eventRepository.Insert(Username, newEvent);
            return Ok(new ItemModel
            {
                Id = newEvent.Id,
                ProductId = newEvent.ProductId,
                Active = true
            });
        }

        [HttpPost]
        [Route("barcode")]
        public IHttpActionResult PostAddByBarcode([FromBody] AddItemByBarcodeApiModel model)
        {
            var product = _productsRepository.Get(Username, p => p.Barcodes.Contains(model.Barcode)).SingleOrDefault();
            if (product == null)
                return NotFound();

            if (_itemsService.GetItems(Username).Where(i => i.Active).Any(i => i.ProductId == product.Id))
                return BadRequest($"{product.Name} finns redan i inköpslistan.");

            var newEvent = new ItemAddedToListEvent(Username, product.Id);
            _eventRepository.Insert(Username, newEvent);
            return Ok(new
            {
                newEvent.Id,
                newEvent.ProductId,
                ProductName = product.Name,
                Active = true
            });
        }

        [HttpPost]
        [Route("readd/{id}")]
        public IHttpActionResult PostReAdd(int id)
        {
            var newEvent = new ItemReaddedToListEvent(Username, id);
            _eventRepository.Insert(Username, newEvent);
            return Ok(newEvent);
        }

        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult Delete(int id)
        {
            var newEvent = new ItemRemovedFromListEvent(Username, id);
            _eventRepository.Insert(Username, newEvent);
            return Ok(newEvent);
        }

        [HttpPut]
        [Route("markasdone/{id}")]
        public IHttpActionResult PutMarkAsDone(int id)
        {
            var newEvent = new ItemMarkedAsDoneEvent(Username, id);
            _eventRepository.Insert(Username, newEvent);
            return Ok(newEvent);
        }

        [HttpPut]
        [Route("comment/{id}")]
        public IHttpActionResult PutComment(int id, [FromBody] SetCommentModel model)
        {
            var newEvent = new CommentSetOnItemEvent(Username, id, model.Comment);
            _eventRepository.Insert(Username, newEvent);
            return Ok();
        }
    }

    public class AddItemApiModel
    {
        public int ProductId { get; set; }
    }

    public class AddItemByBarcodeApiModel
    {
        public string Barcode { get; set; }
    }

    public class SetCommentModel
    {
        public string Comment { get; set; }
    }
}
