
using AutoMapper;
using Empresa.Inv.Application.Shared.Entities;
using Empresa.Inv.Core.Entities;
using Empresa.Inv.Dtos;
using Empresa.Inv.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empresa.Inv.Application
{                                                                      

                                                      
    public class InvAppService : IInvAppService
    {                                                                               

        private readonly IRepository<Product> _productsRepository;
        private readonly IProductCustomRepository _productCustomRepository;

        private readonly IRepository<Supplier> _supplierRepository;
        private readonly IRepository<Category> _categoryRepository;

        private readonly IRepository<ProductKardex> _productKardexesRepository;
        private readonly IRepository<ProductBalance> _productBalances;
        private readonly IUnitOfWork _uow;

        private readonly IMapper _mapper;
        private readonly ILogger<InvAppService> _logger;



        public InvAppService(

            IRepository<Product> productsRepository,
            IRepository<Supplier> supplierRepository,
            IRepository<Category> categoryRepository,
            IRepository<ProductKardex> productKardexesRepository,
            IRepository<ProductBalance> productBalances,         
            IProductCustomRepository  productCustomRepository,

            IMapper mapper,
            ILogger<InvAppService> logger,
            IUnitOfWork uow                                 
            )
        {
            _productCustomRepository = productCustomRepository;

            _productsRepository = productsRepository;
            _supplierRepository = supplierRepository;
            _categoryRepository = categoryRepository;
            _productKardexesRepository = productKardexesRepository;
            _productBalances = productBalances;

            _mapper = mapper;
            _logger = logger;
            _uow = uow;

        }





        public async Task<IEnumerable<ProductDTO>> GetProductsPagedAsyncEf(
            string searchTerm, int pageNumber, int pageSize)
        {
            var query = _productsRepository.GetAll().AsQueryable();

            // Aplicar filtrado
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => p.Name.Contains(searchTerm));
            }

            // Aplicar paginación
            var products = await query
                .OrderBy(p => p.Name) // Ordenar por algún criterio
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProductDTO>>(products);
        }



        public async Task<ProductDTO> GetProductDetailsByIdAsync(int id)
        {
            // Obtener el producto y los datos relacionados en una sola consulta
            var productDto = await _productsRepository.GetAll()
                .Where(p => p.Id == id)
                .Include(p => p.Category)   // Cargar la categoría relacionada
                .Include(p => p.Supplier)   // Cargar el proveedor relacionado
                .Select(p => new ProductDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,  // inspecciona la autorizacion y si le envio
                    CategoryName = string.IsNullOrWhiteSpace(p.Category.Name) ? "No Category" : p.Category.Name, // Validar y proporcionar valor predeterminado
                    SupplierName = string.IsNullOrWhiteSpace(p.Supplier.Name) ? "No Supplier" : p.Supplier.Name  // Validar y proporcionar valor predeterminado
                })
                .FirstOrDefaultAsync();


            if (productDto == null)
            {
                return new ProductDTO();
            }


            return productDto;
        }

        public async Task<Boolean> UpdateInventAsync(int productId, int typeId, decimal amount, int userId)
        {
            bool result = false;

            //reglas del negocio
            if (amount <= 0)
            {
                throw new ArgumentException("La cantidad debe ser mayor que cero.");
            }

            //empezar transaccion
            await _uow.BeginTransactionAsync();


            try
            {
                //Insercion al kardex

                var kardexEntry = new ProductKardex
                {
                    ProductId = productId,
                    Amount = amount,
                    UserId = userId,
                    Created = DateTime.UtcNow,
                    TipoId = typeId

                };
                await _productKardexesRepository.AddAsync(kardexEntry);


                //Actualizacion al Balance
                // Buscar el registro registro que totaliza ese producto

                // Buscar el balance actual del producto
                var productBalance = await _productBalances.GetAll()
                    .Where(pb => pb.ProductId == productId)
                    .FirstOrDefaultAsync();

                if (productBalance != null)
                {
                    switch (typeId)
                    {
                        case 1:
                            productBalance.Amount += amount;
                            productBalance.UserId = userId;
                            productBalance.Created = DateTime.UtcNow;
                            break;

                        case 2:
                            productBalance.Amount -= amount;
                            productBalance.UserId = userId;
                            productBalance.Created = DateTime.UtcNow;
                            break;

                        default:
                            break;
                    }

                    _productBalances.Update(productBalance);   // Marca la entidad para actualización
                }
                else
                {
                    productBalance = new ProductBalance
                    {
                        ProductId = productId,
                        Amount = amount,
                        UserId = userId,
                        Created = DateTime.UtcNow

                    };

                    await _productBalances.AddAsync(productBalance);

                }

                // Guardar los cambios en ProductKardex y ProductBalance
                await _uow.SaveAsync();


                // Confirmar la transacción (commit)
                await _uow.CommitTransactionAsync();
                result = true;

            }
            catch (Exception exx)
            {
                // Si algo falla, revertir los cambios (rollback)
                await _uow.RollbackTransactionAsync();
                throw; // Lanza la excepción para manejarla en capas superiores
            }



            return result;

        }

        public async Task<List<UserKardexSummaryDto>> GetKardexSummaryByUserAsync(DateTime startDate, DateTime endDate)
        {
            var result = await _productKardexesRepository.GetAll()
                .Where(k => k.Created >= startDate && k.Created <= endDate)
                .GroupBy(k => k.UserId)
                .Select(g => new UserKardexSummaryDto
                {
                    UserId = g.Key,
                    CantidadMovimientos = g.Count(),
                    TotalIngresos = g.Sum(k => k.TipoId == 1 ? k.Amount : 0),
                    TotalEgresos = g.Sum(k => k.TipoId == 2 ? k.Amount : 0)
                })
                .ToListAsync();

            return result;
        }




        public async Task<List<ProductDTO>> GetFullProductsAsync(
          string? searchTerm, int pageNumber, int pageSize)
        {
            var query = _productsRepository.GetAll().AsQueryable();

            // Aplicar filtrado
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => p.Name.Contains(searchTerm));
            }

            // Aplicar paginación
            var products = await query
                .Include(p => p.Category)   // Cargar la categoría relacionada
                .Include(p => p.Supplier)   // Cargar el proveedor relacionado
                .OrderBy(p => p.Name)       // Ordenar por algún criterio
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                  .Select(p => new ProductDTO
                  {
                      Id = p.Id,
                      Name = p.Name,
                      Price = p.Price,
                      CategoryName = string.IsNullOrWhiteSpace(p.Category.Name) ? "No Category" : p.Category.Name, // Validar y proporcionar valor predeterminado
                      SupplierName = string.IsNullOrWhiteSpace(p.Supplier.Name) ? "No Supplier" : p.Supplier.Name  // Validar y proporcionar valor predeterminado
                  })
                    .ToListAsync();



            if (products == null) products = new List<ProductDTO>();

            return products;
        }

      
        public async Task<List<ProductDTO>> GetProductsSp( string searchTerm,
                    int pageNumber = 1,  int pageSize = 10)
        {
            var lista = await _productCustomRepository.GetProductsPagedAsyncSp(searchTerm, pageNumber, pageSize);
          
            
            return _mapper.Map<List<ProductDTO>>(lista);

       
        }

        public async Task<List<ProductHDTO>> HGetProductsSp(string searchTerm,
                  int pageNumber = 1, int pageSize = 10)
        {
            var lista = await _productCustomRepository.GetProductsPagedAsyncSp(searchTerm, pageNumber, pageSize);


            return _mapper.Map<List<ProductHDTO>>(lista);


        }


        public async Task<ProductDTO> CreateProductAsync(ProductDTO productDto)
        {
            if (productDto == null)
                throw new ArgumentNullException(nameof(productDto));

            // Mapeo de DTO a entidad
            var product = _mapper.Map<Product>(productDto);

            // Agregar a la base de datos
            await _productsRepository.AddAsync(product);

            // Guardar cambios
            await _uow.SaveAsync();

            // Mapeo de entidad a DTO para el resultado
            var resultDto = _mapper.Map<ProductDTO>(product);

            return resultDto;
        }

        public async Task<ProductDTO> GetProductByIdAsync(int id)
        {
            // Obtener el producto por ID
            var product = await _productsRepository.GetAll()
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                throw new KeyNotFoundException($"Producto con ID {id} no encontrado.");

            // Mapeo de entidad a DTO
            var productDto = _mapper.Map<ProductDTO>(product);

            return productDto;
        }





        public async Task<ProductDTO> UpdateProductAsync(int id, ProductDTO productDto)
        {
            if (productDto == null)
                throw new ArgumentNullException(nameof(productDto));

            // Buscar el producto existente
            var existingProduct = await _productsRepository.GetAll()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (existingProduct == null)
                throw new KeyNotFoundException($"Producto con ID {id} no encontrado.");

            // Actualizar campos
            _mapper.Map(productDto, existingProduct);

            // Actualizar en el repositorio
            _productsRepository.Update(existingProduct);

            // Guardar cambios
            await _uow.SaveAsync();

            // Mapeo de entidad a DTO para el resultado
            var resultDto = _mapper.Map<ProductDTO>(existingProduct);

            return resultDto;
        }



        public async Task<bool> DeleteProductAsync(int id)
        {
            // Buscar el producto existente
            var existingProduct = await _productsRepository.GetAll()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (existingProduct == null)
                throw new KeyNotFoundException($"Producto con ID {id} no encontrado.");

            // Eliminar del repositorio
           await _productsRepository.DeleteAsync(id);

            // Guardar cambios
            await _uow.SaveAsync();

            return true;
        }



    }



}
