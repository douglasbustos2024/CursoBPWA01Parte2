using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Empresa.Inv.Application.Shared.ProductEntity.Commands
{
                                 
    public class CreateProductCommand : IRequest<int>
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int? CategoryId { get; set; }
        public int? SupplierId { get; set; }
    }
}
