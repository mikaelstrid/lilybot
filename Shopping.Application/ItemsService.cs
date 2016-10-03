using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lilybot.Core.Application;
using Lilybot.Shopping.Domain;
using Lilybot.Shopping.Domain.Events;

namespace Lilybot.Shopping.Application
{
    public interface IItemsService
    {
        IEnumerable<ItemModel> GetItems(string username);
    }

    public class ItemsService : IItemsService
    {
        private readonly IAggregateRepository<Product> _productsRepository;
        private readonly IEventRepository _eventRepository;

        public ItemsService(IAggregateRepository<Product> productsRepository, IEventRepository eventRepository)
        {
            _productsRepository = productsRepository;
            _eventRepository = eventRepository;
        }

        public IEnumerable<ItemModel> GetItems(string username)
        {
            var allEvents = _eventRepository.GetAll(username).OrderBy(e => e.TimestampUtc);

            var allProducts = _productsRepository.GetAll(username).ToLookup(p => p.Id);

            var items = new List<ItemModel>();

            foreach (var @event in allEvents)
            {
                if (@event is ItemAddedToListEvent)
                {
                    var addEvent = @event as ItemAddedToListEvent;
                    items.Add(new ItemModel
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

            return items;
        }
    }

    public class ItemModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public bool Active { get; set; }
        public string Comment { get; set; }
    }
}
