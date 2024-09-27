
using Empresa.Inv.Core.Entities;
using Empresa.Inv.Dtos;

namespace Empresa.Inv.EntityFrameworkCore
{
    public interface IProductCustomRepository : IRepository<Product>
    {

        // Métodos adicionales si es necesario
                       
         Task<IEnumerable<ProductDTO>> GetProductsPagedAsyncSp(string searchTerm, int pageNumber, int pageSize);


    }



}
