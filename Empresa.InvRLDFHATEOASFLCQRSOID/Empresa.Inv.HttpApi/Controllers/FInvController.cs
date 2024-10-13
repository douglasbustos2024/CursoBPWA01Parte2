using Microsoft.AspNetCore.Mvc;
using Empresa.Inv.Application.Shared.Entities;
using Empresa.Inv.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;
using Empresa.Inv.Application.Shared.Entities.Dtos;
using AutoMapper;
using FluentValidation;
using Microsoft.ApplicationInsights;

namespace Empresa.Inv.HttpApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FInvController : ControllerBase
    {
        private readonly IInvAppService _productsAppService;
                                                           

        private readonly TelemetryClient _telemetryClient;

        private readonly IValidator<ProductDto> _productValidator;

        public FInvController(IInvAppService productsAppService,        
            IValidator<ProductDto> productValidator   ,
            TelemetryClient telemetryClient
            )
        {
            _productsAppService = productsAppService;
            
            _productValidator = productValidator;
            _telemetryClient = telemetryClient;

        }

        // ==========================
        // ACTIONS FOR RESOURCE COLLECTION (PRODUCTS)
        // ==========================


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
            return Ok(new { Products = productList });
        }



        // Create a new product
        [HttpPost("CreateProduct")]
        public async Task<IActionResult> CreateProduct([FromBody] ProductDto productDto)
        {
            if (productDto == null)
            {
                return BadRequest("Request body cannot be null.");
            }


            // Validating the productDto using FluentValidation
            var validationResult = await _productValidator.ValidateAsync(productDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
                                                                          

            // Call the service to create the product
            var product = await _productsAppService.CreateProductAsync(productDto);

      

            // Return the newly created resource with the links
            return CreatedAtAction(nameof(GetProductById), new { id = product.Id } );
        }

        // ==========================
        // ACTIONS FOR INDIVIDUAL RESOURCES (PRODUCTS)
        // ==========================

        // Get a product by its ID with HATEOAS
        [HttpGet("GetProductById/{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {

            _telemetryClient.TrackEvent("MyCustomEvent");


            var product = await _productsAppService.GetProductDetailsByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            

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

                

    }
}
