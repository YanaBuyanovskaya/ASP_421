using ASP_421.Data;
using ASP_421.Data.Entities;
using ASP_421.Services.KDF;
using ASP_421.Services.Random;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IRandomService, DefaultRandomService>();

builder.Services.AddSingleton<TimeStampService>();

builder.Services.AddSingleton<IKDFService, PbKDFService>();

builder.Services.AddDbContext<DataContext>(options =>
options.UseSqlServer(
    builder.Configuration.GetConnectionString("DataContext"))
);







builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<ASP_421.Infasctructure.VisitLogger>();

builder.Services.AddControllersWithViews(o =>
    {
        o.Filters.Add<ASP_421.Infasctructure.VisitLogger>();
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
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
