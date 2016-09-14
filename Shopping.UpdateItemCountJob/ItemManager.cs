using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Lilybot.Core.Application;
using Lilybot.Core.Domain.Model;
using Lilybot.Shopping.Domain;
using Lilybot.Shopping.Domain.Events;

namespace Lilybot.Shopping.UpdateItemCountJob
{
    public interface IItemManager
    {
        void UpdateItemCounts();
    }

    public class ItemManager : IItemManager
    {
        private readonly IAggregateRepository<Product> _productsRepository;
        private readonly IEventRepository _eventRepository;
        private readonly TextWriter _log;

        public ItemManager(IAggregateRepository<Product> productsRepository, IEventRepository eventRepository, TextWriter log)
        {
            _productsRepository = productsRepository;
            _eventRepository = eventRepository;
            _log = log;
        }

        public void UpdateItemCounts()
        {
            var allProductsEnumerable = _productsRepository.GetAll();
            var allProductsArray = allProductsEnumerable as Product[] ?? allProductsEnumerable.ToArray();

            var allEventsEnumerable = _eventRepository.GetFrom("*", allProductsArray.Min(p => p.CountUpdateTimestampUtc));
            var allEventsList = allEventsEnumerable as Event[] ?? allEventsEnumerable.ToArray();
            var snapshotTimeUtc = DateTime.UtcNow;

            foreach (var product in allProductsArray)
            {
                try
                {
                    var addedCount = allEventsList.Count(@event =>
                        @event is ItemAddedToListEvent
                        && ((ItemAddedToListEvent)@event).ProductId == product.Id);

                    product.Count += addedCount;
                    product.CountUpdateTimestampUtc = snapshotTimeUtc;
                    _productsRepository.Update(product);

                    _log.WriteLine($"{product.Name}: {product.Count - addedCount}->{product.Count}");

                }
                catch (Exception e)
                {
                    _log.WriteLine($"Error: {e}");
                }
            }
        }
    }
}