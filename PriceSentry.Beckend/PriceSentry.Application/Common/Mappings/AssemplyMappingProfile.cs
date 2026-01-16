using AutoMapper;
using System.Reflection;

namespace PriceSentry.Application.Common.Mappings {
    public class AssemplyMappingProfile : Profile {
        public AssemplyMappingProfile(Assembly assembly) => 
            ApplyMappingsFormAssembly(assembly);

        private void ApplyMappingsFormAssembly(Assembly assembly) {
            var types = assembly.GetExportedTypes()
                .Where(type => type.GetInterfaces()
                                   .Any(i => i.IsGenericType && 
                                             i.GetGenericTypeDefinition() == typeof(IMapWith<>)))
                .ToList();

            foreach (var type in types) { 
                var instence = Activator.CreateInstance(type);
                var methodInfo = type.GetMethod("Mapping");
                methodInfo?.Invoke(instence, new object[] {this});
            }
        }
    }
}
