using ASP_421.Data;
using ASP_421.Data.Entities;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ASP_421.Infasctructure
{
    public class VisitLogger: IAsyncActionFilter
    {
        private readonly DataContext _db;
        public VisitLogger(DataContext db) => _db = db;

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var started = DateTime.Now;

            var executed = await next();

            var http = executed.HttpContext;

            var path = http.Request.Path.ToString();

            String login =
                http.User?.Identity?.IsAuthenticated == true
                ? http.User.Identity!.Name!
                : (http.Session.GetString("Login") ?? "Anonim");

            var log = new Request
            {
                Id = Guid.NewGuid(),
                Time = started,
                Path = path,
                Login = login,
                Answer = http.Response.StatusCode
            };

            try
            {
                _db.Requests.Add(log);
                await _db.SaveChangesAsync();
            }
            catch(Exception ex) 
            {
                Console.WriteLine(ex);
            }

        }
    }
}
