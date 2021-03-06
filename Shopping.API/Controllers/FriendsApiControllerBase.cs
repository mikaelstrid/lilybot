using System;
using System.Collections.Generic;
using System.Web.Http;
using AutoMapper;
using Lilybot.Shopping.Domain;

namespace Lilybot.Shopping.API.Controllers
{
    public abstract class FriendsApiControllerBase : ApiController
    {
        protected readonly IMapper DefaultMapper;

        protected FriendsApiControllerBase()
        {
            DefaultMapper = CreateMapper();
        }

        protected string Username => ActionContext.Request.Properties["username"].ToString();

        protected static IMapper CreateMapper(Action<IMapperConfiguration> extraInitializeAction = null)
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Store, StoreDto>();
                cfg.CreateMap<StoreSection, StoreSectionDto>();
                cfg.CreateMap<Product, ProductDto>()
                    .ForMember(
                        dest => dest.Barcodes, 
                        m => m.MapFrom(src => (src.Barcodes ?? "").Split(new [] {';'}, StringSplitOptions.RemoveEmptyEntries)));
                extraInitializeAction?.Invoke(cfg);
            });
            return mapperConfiguration.CreateMapper();
        }
    }


    public class StoreDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<StoreSectionDto> Sections { get; set; }
        public ICollection<ProductDto> IgnoredProducts { get; set; }
    }

    public class StoreSectionDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<ProductDto> Products { get; set; }
    }

    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public List<string> Barcodes { get; set; }
    }

    public class EventDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

}