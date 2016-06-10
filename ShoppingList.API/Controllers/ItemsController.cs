using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Lily.Core.Application;
using Lily.Core.Domain.Model;
using Lily.ShoppingList.Domain;

namespace Lily.ShoppingList.Api.Controllers
{
    [Authorize]
    [RoutePrefix("api/items")]
    public class ItemsController : ApiController
    {
        private readonly IAggregateRepository<Product> _productsRepository;
        private readonly IAggregateRepository<AddItemToListEvent> _addItemToListEventRepository;
        private readonly IAggregateRepository<ReAddItemToListEvent> _reAddItemToListEventRepository;
        private readonly IAggregateRepository<RemoveItemFromListEvent> _removeItemToListEventRepository;
        private readonly IAggregateRepository<MarkItemAsDoneEvent> _markItemAsDoneEventRepository;
        private readonly IAggregateRepository<SetCommentEvent> _setCommentEventRepository;

        public ItemsController(
            IAggregateRepository<Product> productsRepository,
            IAggregateRepository<AddItemToListEvent> addItemToListEventRepository,
            IAggregateRepository<ReAddItemToListEvent> reAddItemToListEventRepository,
            IAggregateRepository<RemoveItemFromListEvent> removeItemToListEventRepository,
            IAggregateRepository<MarkItemAsDoneEvent> markItemAsDoneEventRepository,
            IAggregateRepository<SetCommentEvent> setCommentEventRepository)
        {
            _productsRepository = productsRepository;
            _addItemToListEventRepository = addItemToListEventRepository;
            _reAddItemToListEventRepository = reAddItemToListEventRepository;
            _removeItemToListEventRepository = removeItemToListEventRepository;
            _markItemAsDoneEventRepository = markItemAsDoneEventRepository;
            _setCommentEventRepository = setCommentEventRepository;
        }

        [HttpGet]
        [Route("active")]
        public async Task<IHttpActionResult> GetActive()
        {
            var allEvents = (await _addItemToListEventRepository.GetAll(User.Identity.Name) as IEnumerable<Event>)
                .Concat(await _reAddItemToListEventRepository.GetAll(User.Identity.Name))
                .Concat(await _removeItemToListEventRepository.GetAll(User.Identity.Name))
                .Concat(await _markItemAsDoneEventRepository.GetAll(User.Identity.Name))
                .Concat(await _setCommentEventRepository.GetAll(User.Identity.Name))
                .OrderBy(e => e.TimestampUtc);

            var allProducts = (await _productsRepository.GetAll(User.Identity.Name)).ToLookup(p => p.Id);

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
        public async Task<IHttpActionResult> Post([FromBody] AddItemApiModel model)
        {
            var newEvent = new AddItemToListEvent(User.Identity.Name) { ProductId = model.ProductId };
            await _addItemToListEventRepository.AddOrUpdate(User.Identity.Name, newEvent);
            return Ok(newEvent);
        }

        [HttpPost]
        [Route("readd/{id}")]
        public async Task<IHttpActionResult> PostReAdd(Guid id)
        {
            var newEvent = new ReAddItemToListEvent(User.Identity.Name) { OldItemId = id };
            await _reAddItemToListEventRepository.AddOrUpdate(User.Identity.Name, newEvent);
            return Ok(newEvent);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete(Guid id)
        {
            var newEvent = new RemoveItemFromListEvent(User.Identity.Name) { ItemId = id };
            await _removeItemToListEventRepository.AddOrUpdate(User.Identity.Name, newEvent);
            return Ok(newEvent);
        }

        [HttpPut]
        [Route("markasdone/{id}")]
        public async Task<IHttpActionResult> PutMarkAsDone(Guid id)
        {
            var newEvent = new MarkItemAsDoneEvent(User.Identity.Name) { ItemId = id };
            await _markItemAsDoneEventRepository.AddOrUpdate(User.Identity.Name, newEvent);
            return Ok(newEvent);
        }

        [HttpPut]
        [Route("comment/{id}")]
        public async Task<IHttpActionResult> PutComment(Guid id, [FromBody] SetCommentModel model)
        {
            var newEvent = new SetCommentEvent(User.Identity.Name) { ItemId = id, Comment = model.Comment };
            await _setCommentEventRepository.AddOrUpdate(User.Identity.Name, newEvent);
            return Ok();
        }
    }

    public class GetItemApiModel
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string Comment { get; set; }
        public bool Active { get; set; }
    }

    public class AddItemApiModel
    {
        public Guid ProductId { get; set; }
    }

    public class SetCommentModel
    {
        public string Comment { get; set; }
    }
}
