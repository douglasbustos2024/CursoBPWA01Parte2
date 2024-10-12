

using AutoMapper;
using Empresa.Inv.Application.Shared.ProductEntity.Commands;
using Empresa.Inv.Core.Entities;
using Empresa.Inv.Dtos;

namespace Empresa.Inv.EntityFrameworkCore
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Configuración de mapeo para Product
            CreateMap<Product, ProductDto>();

            CreateMap<ProductDto, Product>();

            CreateMap<ProductHmDto, Product>().ReverseMap();

            CreateMap<ProductHmDto, ProductDto>().ReverseMap();
             

            CreateMap<UserDto, User>().ReverseMap();        

            CreateMap<CreateProductCommand, Product>();
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : "No Category"))
                .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.Name : "No Supplier"));




        }
    }
}
