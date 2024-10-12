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
        Task<List<ProductDto>> GetFullProductsAsync(string searchTerm, int pageNumber, int pageSize);
        Task<IEnumerable<ProductDto>> GetProductsPagedAsyncEf(string searchTerm, int pageNumber, int pageSize);

        Task<ProductDto> GetProductDetailsByIdAsync(int id);

        Task<Boolean> UpdateInventAsync(int productId, int typeId, decimal amount, int userId);

        Task<List<UserKardexSummaryDto>> GetKardexSummaryByUserAsync(DateTime startDate, DateTime endDate);

        Task<List<ProductDto>> GetProductsSp(string searchTerm,int pageNumber = 1, int pageSize = 10);
        Task<List<ProductHmDto>> HGetProductsSp(string searchTerm,int pageNumber = 1, int pageSize = 10);




        Task<ProductDto> CreateProductAsync(ProductDto productDto);
        Task<ProductDto> GetProductByIdAsync(int id);
        Task<ProductDto> UpdateProductAsync(int id, ProductDto productDto);

        Task<bool> DeleteProductAsync(int id);


    }


}
