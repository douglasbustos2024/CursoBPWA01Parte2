using Empresa.Inv.Dtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empresa.Inv.Application.Validators
{
    public class ProductValidator : AbstractValidator<ProductDTO>
    {
        public ProductValidator()
        {
            RuleFor(x => x.Name)
          .NotEmpty().WithMessage("El nombre del producto es obligatorio.")
          .Length(3, 100).WithMessage("El nombre debe tener entre 3 y 100 caracteres.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("El precio debe ser mayor que 0.");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("La categoría es obligatoria.");

            RuleFor(x => x.SupplierId)
                .NotEmpty().WithMessage("El proveedor es obligatorio.");
        }

    }
}
