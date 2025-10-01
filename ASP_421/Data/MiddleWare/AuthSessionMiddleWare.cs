using ASP_421.Data.Entities;
using System.Globalization;
using System.Security.Claims;
using System.Text.Json;

namespace ASP_421.Data.MiddleWare
{
    public class AuthSessionMiddleWare
    {

        private readonly RequestDelegate _next;

        public AuthSessionMiddleWare(RequestDelegate next)

        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)

        {
            if (context.Request.Query.ContainsKey("logout"))
            {
                context.Session.Remove("SignIn");
                context.Response.Redirect(context.Request.Path);
                return;
            }

            if (context.Session.Keys.Contains("SignIn"))
            {
                UserAccess userAccess = 
                    JsonSerializer.Deserialize<UserAccess>(
                    context.Session.GetString("SignIn")!)!;

                context.User = new ClaimsPrincipal(
                    new ClaimsIdentity
                    (
                        [
                            new Claim(ClaimTypes.Name, userAccess.User.Name),
                            new Claim(ClaimTypes.Email, userAccess.User.Email),
                            new Claim("Id", userAccess.User.Id.ToString()),
                            new Claim(ClaimTypes.NameIdentifier, userAccess.Login),
                            new Claim(ClaimTypes.Role, userAccess.RoleId)
                    ],
                        nameof(AuthSessionMiddleWare)
                    )
                    );
            }
            await _next(context);
        }

      
    }

    public static class AuthSessionMiddlewareExtensions

    {
        public static IApplicationBuilder UseAuthSession(

            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthSessionMiddleWare>();
        }

    }
}
