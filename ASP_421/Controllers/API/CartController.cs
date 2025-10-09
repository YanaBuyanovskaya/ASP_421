using ASP_421.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ASP_421.Controllers.API
{
    [Route("api/cart")]
    [ApiController]
    public class CartController (DataAccessor dataAccessor) : ControllerBase
    {
        private readonly DataAccessor _dataAccessor = dataAccessor;
        [Authorize]
        [HttpGet("summary")]
        public IActionResult Summary()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue(ClaimTypes.PrimarySid);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { Status = "Unauthorized" });

            var total = _dataAccessor.GetCartTotalQty(userId);
            return Ok(new { totalQty = total });
        }
        [HttpPost("{id:guid}")]
        public IActionResult AddProductToCart(Guid id)
        {
            //задача перевірити чи запит авторизований
            
                //задача вилучити дані про авторизацію з контексту HTTP
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? User.FindFirstValue(ClaimTypes.PrimarySid);
                //передати роботу на DataAccessor

                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { Status = "Unauthorized" });

                try
                {
                    _dataAccessor.AddProductToCart(userId, id.ToString());
                var total = _dataAccessor.GetCartTotalQty(userId);    
                return Ok(new 
                    { 
                    Code=200,
                    Status="Ok",
                    });
                    
                }
                catch (ArgumentException ex)
                {
                    return BadRequest(new
                    {
                        Code = 400,
                        Status = "Error data validation",
                        message = ex.Message,
                    });
                }
                catch(KeyNotFoundException ex)
                {
                    return NotFound(new
                    {
                        Status = "Not found",
                        message = ex.Message
                    });
                }
                catch(Exception)
                {
                    return StatusCode(500, new
                    {
                        Code= 500,
                        Status = "Error"
                    });
                }
            }  
        }
    
    }


