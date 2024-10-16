﻿using Empresa.Inv.Application.Shared.Entities.Dtos;
using System.Net.Mail;

namespace Empresa.Inv.Dtos
{
    public class ProductHmResourceDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }

        public int CategoryId { get; set; }

        public string? CategoryName { get; set; }

        public int SupplierId { get; set; }
        public string? SupplierName { get; set; }

        public List<LinkResourceDto> Enlaces { get; set; } = new();

    }
}
