

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
            CreateMap<Product, ProductDTO>();

            CreateMap<ProductDTO, Product>();

            CreateMap<ProductHDTO, Product>().ReverseMap();

            CreateMap<ProductHDTO, ProductDTO>().ReverseMap();
             

            CreateMap<UserDTO, User>().ReverseMap();


            // Puedes agregar más configuraciones de mapeo aquí
            // Ejemplo:
            // CreateMap<OtherEntity, OtherDTO>();
            // CreateMap<OtherDTO, OtherEntity>();


            CreateMap<CreateProductCommand, Product>();
            CreateMap<Product, ProductDTO>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : "No Category"))
                .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.Name : "No Supplier"));




        }
    }
}
