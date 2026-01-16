using AutoMapper;

namespace PriceSentry.Application.Common.Mappings {
    public interface IMapWith<T> {
        void Mapping(Profile profile) => profile.CreateMap(typeof(T), GetType());
    }
}
