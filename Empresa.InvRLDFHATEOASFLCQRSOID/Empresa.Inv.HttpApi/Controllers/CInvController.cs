using Microsoft.AspNetCore.Mvc;
using Empresa.Inv.Application.Shared.Entities;
using Empresa.Inv.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;
using Empresa.Inv.Application.Shared.Entities.Dtos;
using AutoMapper;
using MediatR;
using Empresa.Inv.Application.Shared.ProductEntity.Commands;
using Empresa.Inv.Application.Shared.ProductEntity.Queries;

namespace Empresa.Inv.HttpApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CInvController : ControllerBase
    {
        private readonly IInvAppService _productsAppService;
  
     
        private readonly IMediator _mediator;

        public CInvController(IInvAppService productsAppService,   
            IMapper mapper   , IMediator mediator
            )
        {
            _productsAppService = productsAppService;
 
            
            _mediator = mediator;
        }

      


        [HttpGet("GetProductsSp")]
        public async Task<IActionResult> GetProductsSp([FromQuery] string searchTerm, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            // Obtiene los productos del AppService
            var productList = await _productsAppService.GetProductsSp(searchTerm, pageNumber, pageSize);

            // Verifica si la lista de productos es nula o vacía
            if (productList == null || !productList.Any())
            {
                return NotFound("No products found.");
            }

          
      

            // Retorna los productos y los enlaces HATEOAS
            return Ok(new { productList });
        }



        

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
        {
            var productId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetProductById), new { id = productId }, null);
        }



        // GET api/products/{id}
        [HttpGet("GetProductById/{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var query = new GetProductByIdQuery(id);
            var product = await _mediator.Send(query);
            if (product == null)
                return NotFound();
            return Ok(product);
        }


 

        // Update a product by its ID
        [HttpPut("UpdateProduct/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductDto productDto)
        {
            if (id != productDto.Id)
            {
                return BadRequest("Product ID does not match the URL parameter.");
            }

            var updatedProduct = await _productsAppService.UpdateProductAsync(id, productDto);
            return Ok(updatedProduct);
        }

        // Delete a product by its ID
        [HttpDelete("DeleteProduct/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _productsAppService.DeleteProductAsync(id);
            if (!result)
            {
                return NotFound($"Product with ID {id} not found.");
            }

            return NoContent();
        }

           
    }
}
