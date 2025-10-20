using ASP_421.Data;
using System.Security.Claims;

namespace ASP_421.Middleware
{
    public class UserCartMiddleware
    {
        private readonly RequestDelegate _next;

        public UserCartMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // Інжекція до Middleware здійснюється через параметри Invoke,
        // оскільки конструктор виходить за життєвий цикл сервісів
        public async Task InvokeAsync(HttpContext context, DataAccessor dataAccessor)
        {
            if (context.User.Identity?.IsAuthenticated ?? false)
            {
                // context.Items - словник (ключ-значення) для потреб користувача
                context.Items.Add("UserCart",
                    dataAccessor.GetActiveCart(context
                        .User
                        .Claims
                        .First(c => c.Type == ClaimTypes.PrimarySid)
                        .Value)
                );
            }
            await _next(context);
        }
    }


    public static class UserCartMiddlewareExtensions
    {
        public static IApplicationBuilder UseUserCart(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UserCartMiddleware>();
        }
    }
}

