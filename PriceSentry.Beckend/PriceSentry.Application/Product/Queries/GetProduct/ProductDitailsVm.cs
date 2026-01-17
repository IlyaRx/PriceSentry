
using AutoMapper;
using PriceSentry.Application.Common.Mappings;
using PriceSentry.Application.Product.Queries.GetListProducts;
using PriceSentry.Domain;

namespace PriceSentry.Application.Product.Queries.GetProduct {
    public class ProductDitailsVm : IMapWith<TrackingProduct>{
        public Guid Id { get; set; }
        public required decimal DesiredPrice { get; set; }
        public required string ProductUrl { get; set; }
        public decimal ActualPrice { get; set; }
        public string? Title { get; set; }
        public DateTime? LastTracking { get; set; }

        public void Mapping(Profile profile) {
            profile.CreateMap<TrackingProduct, ProductDitailsVm>()
                .ForMember(prodVm => prodVm.Id, opt => opt.MapFrom(product => product.Id))
                .ForMember(prodVm => prodVm.ActualPrice, opt => opt.MapFrom(product => product.ActualPrice))
                .ForMember(prodVm => prodVm.Title, opt => opt.MapFrom(product => product.Title))
                .ForMember(prodVm => prodVm.ProductUrl, opt => opt.MapFrom(product => product.ProductUrl))
                .ForMember(prodVm => prodVm.LastTracking, opt => opt.MapFrom(product => product.LastTracking))
                .ForMember(prodVm => prodVm.DesiredPrice, opt => opt.MapFrom(product => product.DesiredPrice));
        }
    }
}
