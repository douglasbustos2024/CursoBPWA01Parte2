﻿using Empresa.Inv.Dtos;
using MediatR;

namespace Empresa.Inv.Application.Shared.ProductEntity.Queries
{
  

    public class GetProductByIdQuery : IRequest<ProductDto>
    {
        public int Id { get; set; }

        public GetProductByIdQuery(int id)
        {
            Id = id;
        }
    }
}
