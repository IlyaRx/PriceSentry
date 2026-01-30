
using AutoMapper;
using PriceSentry.Application.Common.Mappings;
using PriceSentry.Application.Product.Queries.GetActualPrice;
using PriceSentry.Domain;

namespace PriceSentry.Application.Product.Queries.GetListProducts {
    public class ProductLookupVm : IMapWith<TrackingProduct>{
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public required string ProductUrl { get; set; }
        public decimal ActualPrice { get; set; }
        public decimal DesiredPrice { get; set; }

        public void Mapping(Profile profile) {
            profile.CreateMap<TrackingProduct, ProductLookupVm>()
                .ForMember(prodVm => prodVm.Id, opt => opt.MapFrom(product => product.Id))
                .ForMember(prodVm => prodVm.ActualPrice, opt => opt.MapFrom(product => product.ActualPrice))
                .ForMember(prodVm => prodVm.Title, opt => opt.MapFrom(product => product.Title))
                .ForMember(prodVm => prodVm.DesiredPrice, opt => opt.MapFrom(product => product.DesiredPrice))
                .ForMember(prodVm => prodVm.ProductUrl, opt => opt.MapFrom(product => product.ProductUrl));
        }
    }
}
