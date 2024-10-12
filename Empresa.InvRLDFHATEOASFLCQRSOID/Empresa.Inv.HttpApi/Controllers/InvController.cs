using Microsoft.AspNetCore.Mvc;     
using Empresa.Inv.HttpApi.Services;


using Empresa.Inv.Application.Shared.Entities;
using Empresa.Inv.Dtos;

namespace Empresa.Inv.HttpApi.Controllers
{



    [ApiController]
    //  [Authorize]
    [Route("api/[controller]")]
    public class InvController : ControllerBase
    {
        private readonly IInvAppService _productsAppService;
                                                                  
    


        public InvController(
            IInvAppService  productsAppService 

            )
        {
             

            _productsAppService =  productsAppService;
                                           
        

        }


        [HttpGet("GetProductNames/{id}")]
        public async Task<IActionResult> GetProductNames(int id)
        {

            ProductDto resultado = new ProductDto();
            resultado = await _productsAppService.GetProductDetailsByIdAsync(id);

            return Ok(resultado);


        }


        [HttpGet("ProductsSp")]
        public async Task<IActionResult> GetProductsSp(
           [FromQuery] string searchTerm,
           [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var lista = await _productsAppService.GetProductsSp(searchTerm, pageNumber, pageSize);


            return Ok(lista);
        }


                      



    }


}
