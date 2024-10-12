using Microsoft.AspNetCore.Mvc;
using Empresa.Inv.HttpApi.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Empresa.Inv.HttpApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CacheController : ControllerBase
    {
        private readonly CacheService _cacheService;

        public CacheController(CacheService cacheService)
        {
            _cacheService = cacheService;
        }

        /// <summary>
        /// Limpiar una entrada de caché específica basándose en la clave proporcionada.
        /// </summary>
        /// <param name="key">Clave del cache a limpiar.</param>
        /// <returns>Un mensaje indicando el estado de la operación.</returns>
        [HttpPost("clear-cache")]
    
        public IActionResult ClearCache([FromQuery] string key)
        {

            ObjectResult returned;
            if (string.IsNullOrEmpty(key))
            {
                returned = StatusCode(StatusCodes.Status400BadRequest, new { isSuccess = true, Message = "Cache key must be provided." });
            }
                                    
            try
            {                  
                _cacheService.ClearCache(key);

            }
            catch
            {
                returned = StatusCode(StatusCodes.Status400BadRequest, new { isSuccess = true, Message = "Error" });
            }
                                         

            returned = StatusCode(StatusCodes.Status200OK, new { isSuccess = true, Message = "Cache cleared for key: {key}" });

            return returned;
           
        }

        /// <summary>
        /// Limpiar todos los caches.
        /// </summary>
        /// <returns>Un mensaje indicando el estado de la operación.</returns>
        [HttpPost("clear-all-caches")]
        public IActionResult ClearAllCaches()
        {
            ObjectResult returned;

            try
            {
                _cacheService.ClearAllCaches();

            }
            catch
            {
                returned = StatusCode(StatusCodes.Status400BadRequest, new { isSuccess = true, Message = "Error" });
            }


            returned = StatusCode(StatusCodes.Status200OK, new { isSuccess = true, Message = "All caches cleared." });

            return returned;

                                                     
        }
    }
}
