using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace ASP_421.Controllers
{
    [Route("auth")]
    public class AuthController : Controller
    {
        private bool TryGetUser(string login, string password,
            out (string Name, string Email, DateTime BirthDate) user)
        {
            if (login == "Admin" && password == "Admin")
            {
                user = ("Root Administrator", "admin1@gmail.com", new DateTime(2000, 01, 01));
                return true;
            }
            user = default;
            return false;

        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn()
        {//1
            if (!Request.Headers.TryGetValue("Authorization", out var h) || string.IsNullOrWhiteSpace(h))
                return Error("Відсутній заголовок Authorization");
            var header = h.ToString();
            if (!header.StartsWith("Basic", StringComparison.OrdinalIgnoreCase))
                return Error("Очікується схема Basic");
            //2
            var base64 = header["Basic".Length..].Trim();
            if (string.IsNullOrWhiteSpace(base64))
                return Error("Порожні credentials");

            //3
            byte[] bytes;
            try { bytes = Convert.FromBase64String(base64); }
            catch { return Error("Помилка декодування Base64"); }

            string decoded;
            try { decoded = Encoding.UTF8.GetString(bytes); }
            catch { return Error("Непридатне кодування credentials"); }

            //4
            var idx = decoded.IndexOf(':');
            if (idx <= 0 || idx == decoded.Length - 1)
                return Error("Невірний формат credentials (очікується login:password)");

            var login = decoded[..idx];
            var password = decoded[(idx + 1)..];

            //5
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
                return Error("Логін/пароль не можуть бути порожніми");

            //6
            if (!TryGetUser(login, password, out var user))
                return Error("Невірний логін та пароль");

            //7
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Name),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.DateOfBirth, user.BirthDate.ToString("yyyy-MM-dd"))
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

            return Ok(new { success = true });
        }

        [HttpPost("signout")]
        public async Task<IActionResult> SignOutUser()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        private BadRequestObjectResult Error(string message)
            => BadRequest(new { errors = new { Authentication = new[] { message } } });
    }
}
