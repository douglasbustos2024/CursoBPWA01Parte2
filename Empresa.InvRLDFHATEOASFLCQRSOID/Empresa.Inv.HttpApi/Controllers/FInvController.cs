﻿using Microsoft.AspNetCore.Mvc;
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
        private readonly LinkGenerator _linkGenerator;
        private readonly IMapper _mapper;

        private readonly TelemetryClient _telemetryClient;

        private readonly IValidator<ProductDto> _productValidator;

        public FInvController(IInvAppService productsAppService, 
            LinkGenerator linkGenerator,
            IMapper mapper    ,
            IValidator<ProductDto> productValidator   ,
            TelemetryClient telemetryClient
            )
        {
            _productsAppService = productsAppService;
            _linkGenerator = linkGenerator;
            _mapper = mapper;
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

            // Crear una lista de recursos HATEOAS para cada producto
            var resourceList = new List<ProductHmResourceDto>();
            foreach (var product in productList)
            {
               /* var productHDto = _mapper.Map<ProductDTO>(product);*/  // Mapeo de ProductDTO a ProductHDTO
                var resource = CreateProductResource(product);  // Crear recurso HATEOAS
                resourceList.Add(resource);
            }

            // Enlace para crear un nuevo producto (relacionado con la colección)
            var links = new List<LinkResourceDto>
            {
                new LinkResourceDto
                {
                    Href = _linkGenerator.GetPathByAction(action: "CreateProduct", controller: "HInv"),
                    Rel = "create-product",
                    Metodo = "POST"
                }
            };

            // Retorna los productos y los enlaces HATEOAS
            return Ok(new { Products = resourceList, Links = links });
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

            // Create the HATEOAS resource for the new product
            var resource = CreateProductResource(_mapper.Map<ProductDto>(product));

            // Return the newly created resource with the links
            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, resource);
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

            // Create the HATEOAS resource for this specific product
            var resource = CreateProductResource(_mapper.Map<ProductDto>(product));

            return Ok(resource);
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

        // ==========================
        // HELPER METHODS
        // ==========================

        // Create HATEOAS resource for a product
        private ProductHmResourceDto CreateProductResource(ProductDto product)
        {
            var links = GetProductLinks(product.Id);
            return new ProductHmResourceDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Enlaces = links
            };
        }

        // Get the HATEOAS links for a product
        private List<LinkResourceDto> GetProductLinks(int id)
        {
            var links = new List<LinkResourceDto>
            {
                new LinkResourceDto
                {
                    Href = _linkGenerator.GetPathByAction(action: "GetProductById", controller: "HInv", values: new { id }),
                    Rel = "self",
                    Metodo = "GET"
                },
                new LinkResourceDto
                {
                    Href = _linkGenerator.GetPathByAction(action: "UpdateProduct", controller: "HInv", values: new { id }),
                    Rel = "update-product",
                    Metodo = "PUT"
                },
                new LinkResourceDto
                {
                    Href = _linkGenerator.GetPathByAction(action: "DeleteProduct", controller: "HInv", values: new { id }),
                    Rel = "delete-product",
                    Metodo = "DELETE"
                }
            };
            return links;
        }




        [HttpGet]
        [Route("api/testException")]
        public IActionResult TestException()
        {
            try
            {
                // Simular una excepción
                throw new Exception("Test exception");
            }
            catch (Exception ex)
            {
                // Enviar la excepción a Application Insights
                _telemetryClient.TrackException(ex);
                return StatusCode(500, "Exception logged");
            }
        }



    }
}
