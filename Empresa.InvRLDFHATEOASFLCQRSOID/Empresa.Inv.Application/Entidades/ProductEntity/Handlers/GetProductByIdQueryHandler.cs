using AutoMapper;
 
using Empresa.Inv.Application.Shared.ProductEntity.Queries;
using Empresa.Inv.Core.Entities;
using Empresa.Inv.Dtos;
using Empresa.Inv.EntityFrameworkCore;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace Empresa.Inv.Application.Entidades.ProductEntity.Handlers
{
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IMapper _mapper;

        public GetProductByIdQueryHandler(IRepository<Product> productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product =   _productRepository.Query()
                .Include(p => p.Category)
                .Include(p => p.Supplier);

            var result = await product.FirstOrDefaultAsync(p => p.Id == request.Id);

            return _mapper.Map<ProductDto>(result);
        }
    }
 
}
