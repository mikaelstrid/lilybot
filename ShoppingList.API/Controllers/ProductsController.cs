﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Lilybot.Core.Application;
using Lilybot.Shopping.Domain;

// ReSharper disable PossibleMultipleEnumeration

namespace Lilybot.Shopping.API.Controllers
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
            var allProducts = _repository.GetAll(Username);
            var allProductDtos = DefaultMapper.Map<IEnumerable<Product>, IEnumerable<ProductDto>>(allProducts);
            return Ok(allProductDtos);
        }

        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult Get(int id)
        {
            var product = _repository.GetById(Username, id);
            var productDto = DefaultMapper.Map<ProductDto>(product);
            return Ok(productDto);
        }

        [HttpGet]
        [Route("top/{count}")]
        public IHttpActionResult GetTop(int count)
        {
            var topProducts = _repository.Get(Username, orderBy: q => q.OrderByDescending(p => p.Count), count: count);
            var allProductDtos = DefaultMapper.Map<IEnumerable<Product>, IEnumerable<ProductDto>>(topProducts);
            return Ok(allProductDtos);
        }

        [HttpGet]
        [Route("search")]
        public IHttpActionResult GetSearch(string q)
        {
            var matchingProducts = _repository.Get(
                Username,
                predicate: p => p.Name.ToLower().Contains(q.ToLower()),
                orderBy: o => o.OrderByDescending(p => p.Count));
            var matchingProductDtos = DefaultMapper.Map<IEnumerable<Product>, IEnumerable<ProductDto>>(matchingProducts);
            return Ok(matchingProductDtos);
        }
        
        [HttpPost]
        [Route("")]
        public IHttpActionResult Post([FromBody] CreateOrUpdateProductApiModel model)
        {
            var newProduct = new Product(Username)
            {
                Name = model.Name,
                Count = 0,
                CountUpdateTimestampUtc = DateTime.UtcNow
            };
            _repository.InsertOrUpdate(Username, newProduct);
            var newProductDto = DefaultMapper.Map<ProductDto>(newProduct);
            return Ok(newProductDto);
        }

        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult Put(int id, [FromBody] CreateOrUpdateProductApiModel model)
        {
            var product = _repository.GetById(Username, id);
            if (product == null) return BadRequest("No product found with the specified id.");

            product.Name = model.Name;
            _repository.InsertOrUpdate(Username, product);
            var productDto = DefaultMapper.Map<ProductDto>(product);
            return Ok(productDto);
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
