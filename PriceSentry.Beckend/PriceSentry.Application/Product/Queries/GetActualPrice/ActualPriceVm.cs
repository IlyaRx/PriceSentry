
using AutoMapper;
using PriceSentry.Application.Common.Mappings;
using PriceSentry.Domain;

namespace PriceSentry.Application.Product.Queries.GetActualPrice {
    public class ActualPriceVm : IMapWith<TrackingProduct>{
        public Guid Id { get; set; }
        public decimal ActualPrice { get; set; }

        public void Mapping(Profile profile) {
            profile.CreateMap<TrackingProduct, ActualPriceVm>()
                .ForMember(prodVm => prodVm.Id, opt => opt.MapFrom(product => product.Id))
                .ForMember(prodVm => prodVm.ActualPrice, opt => opt.MapFrom(product => product.ActualPrice));
        }
    }
}
