using ASP_421.Data;
using ASP_421.Data.Entities;
using ASP_421.Data.MiddleWare;
using ASP_421.Middleware;
using ASP_421.Services.KDF;
using ASP_421.Services.Random;
using ASP_421.Services.Storage;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IRandomService, DefaultRandomService>();

builder.Services.AddSingleton<TimeStampService>();

builder.Services.AddSingleton<IKDFService, PbKDFService>();
builder.Services.AddSingleton<IStorageService, DiskStorageService>();


builder.Services.AddDbContext<DataContext>(options =>
options.UseSqlServer(
    builder.Configuration.GetConnectionString("DataContext"))
);

builder.Services.AddScoped<DataAccessor>();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(100);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod()));


builder.Services.AddScoped<ASP_421.Infasctructure.VisitLogger>();

builder.Services.AddControllersWithViews(o =>
{
    o.Filters.Add<ASP_421.Infasctructure.VisitLogger>();
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opt =>
    {
        opt.LoginPath = "/";
        opt.AccessDeniedPath = "/";
    });
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors();
app.UseAuthorization();
app.UseSession();


app.UseStaticFiles();


app.UseAuthentication();




app.UseAuthSession();
app.UseUserCart();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();
app.Run();


/* CORS - Cross Origin Resourse Sharing
 * CORS policy - набір обмежень на обмін даними між різними ресурсами.
 * Ресурси вважаються різними (Cross Origin) якщо в них є одна з відмінностей:
 * - протокол (наприклад, http/https)
 * - домен
 * - порт
 * 
 * Якщо відмінність в адресі після домену, то це не вважається Cross Origin
 * 
 * Обмеження:
 * доступ до ресурсів - обмежується заголовком Acess-Control-Allow-Origin
 * у якому зазначаються дозволені ресурси, для загального доступу - "*"
 *
 * передача заголовків запиту - обмежується майже всі заголовки,
 *  зокрема Authorization та Content-Type
 *  використання методів запиту, окрім GET і POST 
 *  
 *  Якщо у запиті є заголовки, або він надсилається іншим методом, то
 *  перед основним запитом надсилається preflight - запит, який передає
 *  метод та заголовки, що вимагають погодження, а також origin
 *  спеціальним методом OPTIONS
 *  
 *  наприклад,
 *  access-control-request-headers: authorization
 *  access-control-request-method: GET
 *  origin: http://localhost:5173
 *  
 *  Дозволяти можна як окремі заголовки/методи, так і всі з них.
 *  Окремий випадок - credentials - дозвіл лише на передачу того, що
 *  необхідно для авторизації
 *
 *
 *
 *
 *
*/

