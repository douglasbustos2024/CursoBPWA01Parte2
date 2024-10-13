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


 
      

           
    }
}
