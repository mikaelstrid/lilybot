﻿using System;
using Lily.Core.Domain.Model;
using Newtonsoft.Json;

namespace Lily.ShoppingList.Domain
{
    public class RemoveItemFromListEvent : Event, IImuttable
    {
        public RemoveItemFromListEvent(string username) : base(username)
        {
        }

        [JsonProperty(PropertyName = "itemId")]
        public Guid ItemId { get; set; }
    }
}