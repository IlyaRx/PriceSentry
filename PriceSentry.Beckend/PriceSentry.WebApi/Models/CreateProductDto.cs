using AutoMapper;
using PriceSentry.Application.Common.Mappings;
using PriceSentry.Application.Product.Commands.Create;

namespace PriceSentry.WebApi.Models {
    public class CreateProductDto : IMapWith<CreateProductCommand> {
        public decimal DesiredPrice { get; set; }
        public required string ProductUrl { get; set; }

        public void Mapping(Profile profile) {
            profile.CreateMap<CreateProductDto, CreateProductCommand>()
                   .ForMember(command => command.DesiredPrice, opt => opt.MapFrom(dto => dto.DesiredPrice))
                   .ForMember(command => command.ProductUrl, opt => opt.MapFrom(dto => dto.ProductUrl));
        }

    }
}
