using AutoMapper;
using PriceSentry.Application.Common.Mappings;
using PriceSentry.Application.Product.Commands.Update;

namespace PriceSentry.WebApi.Models {
    public class UpdateProductDto : IMapWith<UpdateProductCommand> {
        public Guid Id { get; set; }
        public decimal DesiredPrice { get; set; }

        public void Mapping(Profile profile) {
            profile.CreateMap<UpdateProductDto, UpdateProductCommand>()
                   .ForMember(command => command.Id, opt => opt.MapFrom(dto => dto.Id))
                   .ForMember(command => command.DesiredPrice, opt => opt.MapFrom(dto => dto.DesiredPrice));
        }
    }
}
