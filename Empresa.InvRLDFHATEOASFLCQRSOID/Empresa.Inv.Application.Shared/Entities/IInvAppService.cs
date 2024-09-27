using Empresa.Inv.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empresa.Inv.Application.Shared.Entities
{
    public interface IInvAppService
    {
        Task<List<ProductDTO>> GetFullProductsAsync(string searchTerm, int pageNumber, int pageSize);
        Task<IEnumerable<ProductDTO>> GetProductsPagedAsyncEf(string searchTerm, int pageNumber, int pageSize);

        Task<ProductDTO> GetProductDetailsByIdAsync(int id);

        Task<Boolean> UpdateInventAsync(int productId, int typeId, decimal amount, int userId);

        Task<List<UserKardexSummaryDto>> GetKardexSummaryByUserAsync(DateTime startDate, DateTime endDate);

        Task<List<ProductDTO>> GetProductsSp(string searchTerm,int pageNumber = 1, int pageSize = 10);
        Task<List<ProductHDTO>> HGetProductsSp(string searchTerm,int pageNumber = 1, int pageSize = 10);




        Task<ProductDTO> CreateProductAsync(ProductDTO productDto);
        Task<ProductDTO> GetProductByIdAsync(int id);
        Task<ProductDTO> UpdateProductAsync(int id, ProductDTO productDto);

        Task<bool> DeleteProductAsync(int id);


    }


}
