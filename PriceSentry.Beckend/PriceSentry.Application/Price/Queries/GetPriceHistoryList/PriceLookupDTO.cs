
using AutoMapper;
using PriceSentry.Application.Common.Mappings;
using PriceSentry.Domain;

namespace PriceSentry.Application.Price.Queries.GetPriceHistoryList {
    public class PriceLookupDTO : IMapWith<ProductPriceHistory> {
        public decimal Price { get; set; }
        public DateTime AddDate { get; set; }

        public void Mapping(Profile profile) {
            profile.CreateMap<ProductPriceHistory, PriceLookupDTO>()
                   .ForMember(p => p.Price, opt => opt.MapFrom(price => price.Price))
                   .ForMember(p => p.AddDate, opt => opt.MapFrom(price => price.AddDate));
        }
    }
}
