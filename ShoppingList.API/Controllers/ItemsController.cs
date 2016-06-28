using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Lily.Core.Application;
using Lily.ShoppingList.Domain;
using Lily.ShoppingList.Domain.Events;

namespace Lily.ShoppingList.Api.Controllers
{
    [Authorize]
    //[CheckIfFriendActionFilter] Added in the DI/Autofac configuration
    [RoutePrefix("api/items")]
    public class ItemsController : FriendsApiControllerBase
    {
        private readonly IAggregateRepository<Product> _productsRepository;
        private readonly IEventRepository _eventRepository;

        public ItemsController(
            IAggregateRepository<Product> productsRepository,
            IEventRepository eventRepository)
        {
            _productsRepository = productsRepository;
            _eventRepository = eventRepository;
        }

        [HttpGet]
        [Route("active")]
        public IHttpActionResult GetActive()
        {
            var allEvents = _eventRepository.GetAll(Username)
                .OrderBy(e => e.TimestampUtc);

            var allProducts = _productsRepository.GetAll(Username).ToLookup(p => p.Id);

            var items = new List<GetItemApiModel>();

            foreach (var @event in allEvents)
            {
                if (@event is AddItemToListEvent)
                {
                    var addEvent = @event as AddItemToListEvent;
                    items.Add(new GetItemApiModel
                    {
                        Id = addEvent.Id,
                        ProductId = addEvent.ProductId,
                        ProductName = allProducts[addEvent.ProductId].FirstOrDefault()?.Name,
                        Active = true
                    });
                }
                else if (@event is RemoveItemFromListEvent)
                {
                    var removeEvent = @event as RemoveItemFromListEvent;
                    var item = items.FirstOrDefault(i => i.Id == removeEvent.ItemId);
                    if (item != null) item.Active = false;
                }
                else if (@event is MarkItemAsDoneEvent)
                {
                    var markAsDoneEvent = @event as MarkItemAsDoneEvent;
                    var item = items.FirstOrDefault(i => i.Id == markAsDoneEvent.ItemId);
                    if (item != null) item.Active = false;
                }
                else if (@event is SetCommentEvent)
                {
                    var setCommentEvent = @event as SetCommentEvent;
                    var existingItem = items.FirstOrDefault(i => i.Id == setCommentEvent.ItemId);
                    if (existingItem != null) existingItem.Comment = setCommentEvent.Comment;
                }
                else if (@event is ReAddItemToListEvent)
                {
                    var reAddEvent = @event as ReAddItemToListEvent;
                    var item = items.FirstOrDefault(i => i.Id == reAddEvent.OldItemId);
                    if (item != null) item.Active = true;
                }
            }

            return Ok(items.Where(i => i.Active));
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Post([FromBody] AddItemApiModel model)
        {
            var newEvent = new AddItemToListEvent(Username, model.ProductId);
            _eventRepository.Insert(Username, newEvent);
            return Ok(new GetItemApiModel
            {
                Id = newEvent.Id,
                ProductId = newEvent.ProductId,
                Active = true
            });
        }

        [HttpPost]
        [Route("readd/{id}")]
        public IHttpActionResult PostReAdd(int id)
        {
            var newEvent = new ReAddItemToListEvent(Username, id);
            _eventRepository.Insert(Username, newEvent);
            return Ok(newEvent);
        }

        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult Delete(int id)
        {
            var newEvent = new RemoveItemFromListEvent(Username, id);
            _eventRepository.Insert(Username, newEvent);
            return Ok(newEvent);
        }

        [HttpPut]
        [Route("markasdone/{id}")]
        public IHttpActionResult PutMarkAsDone(int id)
        {
            var newEvent = new MarkItemAsDoneEvent(Username, id);
            _eventRepository.Insert(Username, newEvent);
            return Ok(newEvent);
        }

        [HttpPut]
        [Route("comment/{id}")]
        public IHttpActionResult PutComment(int id, [FromBody] SetCommentModel model)
        {
            var newEvent = new SetCommentEvent(Username, id, model.Comment);
            _eventRepository.Insert(Username, newEvent);
            return Ok();
        }
    }

    public class GetItemApiModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Comment { get; set; }
        public bool Active { get; set; }
    }

    public class AddItemApiModel
    {
        public int ProductId { get; set; }
    }

    public class SetCommentModel
    {
        public string Comment { get; set; }
    }
}
