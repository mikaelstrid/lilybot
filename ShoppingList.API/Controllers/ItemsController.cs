﻿using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Lilybot.Core.Application;
using Lilybot.Shopping.Domain;
using Lilybot.Shopping.Domain.Events;

namespace Lilybot.Shopping.API.Controllers
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
                if (@event is ItemAddedToListEvent)
                {
                    var addEvent = @event as ItemAddedToListEvent;
                    items.Add(new GetItemApiModel
                    {
                        Id = addEvent.Id,
                        ProductId = addEvent.ProductId,
                        ProductName = allProducts[addEvent.ProductId].FirstOrDefault()?.Name,
                        Active = true
                    });
                }
                else if (@event is ItemRemovedFromListEvent)
                {
                    var removeEvent = @event as ItemRemovedFromListEvent;
                    var item = items.FirstOrDefault(i => i.Id == removeEvent.ItemId);
                    if (item != null) item.Active = false;
                }
                else if (@event is ItemMarkedAsDoneEvent)
                {
                    var markAsDoneEvent = @event as ItemMarkedAsDoneEvent;
                    var item = items.FirstOrDefault(i => i.Id == markAsDoneEvent.ItemId);
                    if (item != null) item.Active = false;
                }
                else if (@event is CommentSetOnItemEvent)
                {
                    var setCommentEvent = @event as CommentSetOnItemEvent;
                    var existingItem = items.FirstOrDefault(i => i.Id == setCommentEvent.ItemId);
                    if (existingItem != null) existingItem.Comment = setCommentEvent.Comment;
                }
                else if (@event is ItemReaddedToListEvent)
                {
                    var reAddEvent = @event as ItemReaddedToListEvent;
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
            var newEvent = new ItemAddedToListEvent(Username, model.ProductId);
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
