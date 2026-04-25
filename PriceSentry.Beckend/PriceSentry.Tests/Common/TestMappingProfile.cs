
using AutoMapper;
using PriceSentry.Application.Product.Queries.GetActualPrice;
using PriceSentry.Application.Product.Queries.GetListProducts;
using PriceSentry.Application.Product.Queries.GetProduct;
using PriceSentry.Application.Price.Queries.GetPriceHistoryList;
using PriceSentry.Domain;

namespace PriceSentry.Tests.Mappings;

public class TestMappingProfile : Profile {
    public TestMappingProfile() {
        CreateMap<TrackingProduct, ActualPriceVm>()
            .ForMember(vm => vm.Id, opt => opt.MapFrom(p => p.Id))
            .ForMember(vm => vm.ActualPrice, opt => opt.MapFrom(p => p.ActualPrice));

        CreateMap<TrackingProduct, ProductLookupVm>()
            .ForMember(vm => vm.Id, opt => opt.MapFrom(p => p.Id))
            .ForMember(vm => vm.Title, opt => opt.MapFrom(p => p.Title))
            .ForMember(vm => vm.ProductUrl, opt => opt.MapFrom(p => p.ProductUrl))
            .ForMember(vm => vm.ActualPrice, opt => opt.MapFrom(p => p.ActualPrice))
            .ForMember(vm => vm.DesiredPrice, opt => opt.MapFrom(p => p.DesiredPrice));

        CreateMap<TrackingProduct, ProductDitailsVm>()
            .ForMember(vm => vm.Id, opt => opt.MapFrom(p => p.Id))
            .ForMember(vm => vm.DesiredPrice, opt => opt.MapFrom(p => p.DesiredPrice))
            .ForMember(vm => vm.ProductUrl, opt => opt.MapFrom(p => p.ProductUrl))
            .ForMember(vm => vm.ActualPrice, opt => opt.MapFrom(p => p.ActualPrice))
            .ForMember(vm => vm.Title, opt => opt.MapFrom(p => p.Title))
            .ForMember(vm => vm.LastTracking, opt => opt.MapFrom(p => p.LastTracking));

        CreateMap<ProductPriceHistory, PriceLookupDTO>()
            .ForMember(dto => dto.Price, opt => opt.MapFrom(h => h.Price))
            .ForMember(dto => dto.AddDate, opt => opt.MapFrom(h => h.AddDate));
    }
}