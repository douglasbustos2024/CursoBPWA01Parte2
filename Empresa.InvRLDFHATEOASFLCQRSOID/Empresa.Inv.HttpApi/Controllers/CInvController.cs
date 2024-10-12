﻿using Microsoft.AspNetCore.Mvc;
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
        private readonly LinkGenerator _linkGenerator;
     
        private readonly IMediator _mediator;

        public CInvController(IInvAppService productsAppService, 
            LinkGenerator linkGenerator,
            IMapper mapper   , IMediator mediator
            )
        {
            _productsAppService = productsAppService;
            _linkGenerator = linkGenerator;
            
            _mediator = mediator;
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
    }
}
