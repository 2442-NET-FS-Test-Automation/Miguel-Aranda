using AutoMapper;
using Library.ControllerApi.DTOs;
using Library.Data.Entities;

namespace Library.ControllerApi.Mapping;
// This class inherits from AutoMapper's profile - I'm going to choose to just use one profile
// In my app. It's entire purpose is holding the configuration to map DTOs to Models/Entities
public class MappingProfile : Profile {
    public MappingProfile()
    {
        CreateMap<InventoryItem, InventoryDto>()
            .ForCtorParam("Sku", o => o.MapFrom(s => s.Product.Sku))
            .ForCtorParam("Name", o => o.MapFrom(s => s.Product.Name));

            // It is possible for AutoMapper to pick up the mapping implicitly based on matching name/type
            // If it doesn't do what you want - set it like we did in the example above.

            // Note: Right now this only maps one way. Entity/Model (source) => DtO (destination)
            // We can use ReverseMap if we are confident it will pick it up automatically, or
            // just use aother CreateMap going the other way
    }
}