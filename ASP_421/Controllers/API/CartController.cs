using ASP_421.Data;
using ASP_421.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ASP_421.Controllers.Api
{
    [Route("api/cart")]
    [ApiController]
    public class CartController(DataAccessor dataAccessor) : ControllerBase
    {
        private readonly DataAccessor _dataAccessor = dataAccessor;

        [HttpGet("summary")]
        public object Summary()
        {
            return 1;
        }
        [HttpPut]
        public object Checkout()
        {
            return ExecuteAuthority( _dataAccessor.CheckoutCart);
        }

        [HttpDelete]
        public object CancelCart()
        {
            return ExecuteAuthority( _dataAccessor.CancelCart );
        }

        [HttpPatch("{id}")]
        public object ModifyCartItem(String id, int inc)
        {
            return ExecuteAuthority(
                userId => _dataAccessor.ModifyCartItem(userId, id, inc));
        }

        [HttpDelete("remove/{id}")]
        public object RemoveItemFromCart(String id)
        {
            return ExecuteAuthority(
                userId => _dataAccessor.RemoveItemFromCart(userId, id));
        }

        [HttpPost("{id}")]
        public object AddProductToCart(String id)
        {
            return ExecuteAuthority(
                userId => _dataAccessor.AddProductToCart(userId, id));
        }

        [HttpPost("repeat/{id}")]
        public object RepeatCart(String id)
        {
            return ExecuteAuthority(
            userId => _dataAccessor.RepeatCart(userId, id));
        }

        [HttpPost("restore/{id}")]
        public object RestoreCart(String id)
        {
            return ExecuteAuthority(
                userId =>_dataAccessor.RestoreCart(userId, id));
        }

        private object ExecuteAuthority(Action<String> action)
        {
            // Перевірити чи запит авторизований
            if (HttpContext.User.Identity?.IsAuthenticated ?? false)
            {
                // Вилучити дані про авторизацію з контексту HTTP
                String userId = HttpContext.User.Claims
                    .First(c => c.Type == ClaimTypes.PrimarySid).Value;
                // Передати роботу на DataAccessor
                try
                {
                    action(userId);
                    return new
                    {
                        Code = 200,
                        Status = "Ok"
                    };
                }
                catch (Exception ex)
                {
                    return new
                    {
                        Code = 400,
                        Status = "Error data validation",
                        ex.Message,
                    };
                }
            }
            else
            {
                return new
                {
                    Code = 401,
                    Status = "UnAuthorized"
                };
            }
        }

    }
}
/* Д.З. Реалізувати аналіз статусів відповіді сервера на додавання 
 * товару до кошику, виводити відповідні повідомлення користувачу
 * - для замовлення необхідно увійти в систему
 * - товар успішно додано
 * - виникла помилка додавання, повторіть спробу пізніше
 * * якщо немає авторизації, то не надсилати запит, одразу виводити повідомлення (1)
 * 
 * 
 * До сторінки окремого товару: змінити алгоритм вибірки асоціацій
 * (вас також може зацікавити) - обирати 6 позицій: 3 з даної групи
 * (випадково, окрім даного товару) та 3 з інших груп (також випадково)
 */
