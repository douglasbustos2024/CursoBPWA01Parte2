using MediatR;
using AutoMapper;
using Empresa.Inv.EntityFrameworkCore;
using Empresa.Inv.Core.Entities;
using Empresa.Inv.Application.Shared.ProductEntity.Commands;



namespace Empresa.Inv.Application.Entidades.ProductEntity.Handlers
{                                                           
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, int>
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IMapper _mapper;

        public CreateProductCommandHandler(IRepository<Product> productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = _mapper.Map<Product>(request);
            await _productRepository.AddAsync(product);
            return product.Id;
        }
    }

}
