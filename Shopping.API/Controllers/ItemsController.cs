using System.Linq;
using System.Web.Http;
using Lilybot.Core.Application;
using Lilybot.Shopping.Application;
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

        public ItemsController(IEventRepository eventRepository, IItemsService itemsService)
        {
            _eventRepository = eventRepository;
            _itemsService = itemsService;
        }

        [HttpGet]
        [Route("active")]
        public IHttpActionResult GetActive()
        {
            return Ok(_itemsService.GetItems(Username).Where(i => i.Active));
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Post([FromBody] AddItemApiModel model)
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

    public class SetCommentModel
    {
        public string Comment { get; set; }
    }
}
