
using AutoMapper;
using Empresa.Inv.Core.Entities;
using Empresa.Inv.Dtos;
using Empresa.Inv.EntityFrameworkCore.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;


namespace Empresa.Inv.EntityFrameworkCore
{
    public class ProductCustomRepository : Repository<Product>, IProductCustomRepository
    {
        private readonly IMapper _mapper;

        private readonly IUnitOfWork _uow;
                           
        public ProductCustomRepository(ApplicationDbContext context, IUnitOfWork uow,
            IMapper mapper) : base(context)
        {
            _mapper = mapper;
            _uow = uow;
        }
                           

        // Método específico para ejecutar un procedimiento almacenado en el contexto de Product
        public async Task<IEnumerable<ProductDTO>> GetProductsPagedAsyncSp(
            string searchTerm, int pageNumber, int pageSize
            )
        {
             

            var parameters = new[]
            {
                new SqlParameter("@SearchTerm", SqlDbType.NVarChar, 128) { Value = searchTerm ?? (object)DBNull.Value },
                new SqlParameter("@PageNumber", SqlDbType.Int) { Value = pageNumber },
                new SqlParameter("@PageSize", SqlDbType.Int) { Value = pageSize }
            };


            // Ejecutar un procedimiento almacenado que devuelve resultados
            var products = await ExecuteStoredProcedureWithResultsAsync("EXEC GetProductsPaged @SearchTerm,@PageNumber,@PageSize", parameters);

            return _mapper.Map<IEnumerable<ProductDTO>>(products);
            // Trabaja con los resultados
        }
                 





    }

}
