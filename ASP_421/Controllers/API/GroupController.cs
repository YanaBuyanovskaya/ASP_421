using ASP_421.Data;
using ASP_421.Models;
using ASP_421.Models.Shop.API;
using ASP_421.Services.KDF;
using ASP_421.Services.Random;
using ASP_421.Services.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.RegularExpressions;


namespace ASP_421.Controllers.API
{
    [Route("api/group")]
    [ApiController]
    public class GroupController(
        IStorageService storageService,
        DataContext dataContext) 
        : ControllerBase
    {
        private readonly IStorageService _storageService = storageService;
        private readonly DataContext _dataContext = dataContext;
        
        [HttpGet]
        public object AllGroups() // назва методу - довільна, його вибір з GET
        {
            return new { };
        }

       

        [HttpPost]
        public object CreateGroup([FromForm] ShopApiGroupFormModel formModel)
        {

            //валідація моделі - перевірка полів на правильність
            //у т.ч. унікальність Slug

            var errors = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(formModel.Name))
                errors["group-name"] = "Назва обов'язкова";
            else if (formModel.Name.Trim().Length < 2 || formModel.Name.Trim().Length > 100)
                errors["group-name"] = "Довжина назви має бути 2–100 символів";

            if(string.IsNullOrWhiteSpace(formModel.Description))
                errors["group-description"] = "Опис обов'язковий";
            else if(formModel.Description.Trim().Length<10 || formModel.Description.Trim().Length>5000)
                errors["group-description"] = "Довжина опису має бути 10–5000 символів";

            if (formModel.ParentId == Guid.Empty)
                errors["group-parent-id"] = "Некоректний ідентифікатор батьківської групи";
            else if(formModel.ParentId is Guid pid)
            {
                var parentExists = _dataContext.ProductGroups
                    .AsNoTracking()
                    .Any(g => g.Id == pid && g.DeletedAt == null);

                if(!parentExists)
                    errors["group-parent-id"] = "Батьківська група не знайдена";
            }

            if (string.IsNullOrWhiteSpace(formModel.Slug))
                errors["group-slug"] = "Slug обов'язковий";
            else
            {
                var slug = formModel.Slug.Trim();
                if (!Regex.IsMatch(slug, @"^[a-z0-9]+(?:-[a-z0-9]+)*$"))
                {
                    errors["group-slug"] = "Лише нижній регістр, цифри та дефіси: a-z, 0-9, '-'";
                }
                else
                {
                    var exists = _dataContext.ProductGroups
                        .AsNoTracking()
                        .Any(g => g.Slug.ToLower() == slug.ToLower());
                    if (exists)
                        errors["group-slug"] = "Такий slug вже існує";
                }
            }

            if(formModel.Image is null || formModel.Image.Length==0)
            {
                errors["group-image"] = "Завантажте зображення";
            }
            else
            {
                var allowed = new[] { "image/jpeg", "image/jpg", "image/png" };
                if(!allowed.Contains(formModel.Image.ContentType))
                    errors["group-image"] = "Дозволені формати: JPEG, JPG, PNG";
                else if(formModel.Image.Length>2*1024*1024)
                    errors["group-image"] = "Розмір файлу має бути <= 2 МБ"; ;
            }

            if(errors.Count>0)
            {
                Response.StatusCode = 400;
                return new
                {
                    Status = "ValidationError",
                    Errors = errors
                };
            }

                try
                {
                    _dataContext.ProductGroups.Add(new()
                    {
                        Id = Guid.NewGuid(),
                        Name = formModel.Name.Trim(),
                        Description = formModel.Description.Trim(),
                        Slug = formModel.Slug,
                        ParentId = formModel.ParentId,
                        ImageUrl = _storageService.Save(formModel.Image!)
                    });
                    _dataContext.SaveChanges();
                    return new
                    {
                        Status = "Ok"
                    };
                }
                catch (Exception ex)
                {
                    return new
                    {
                        Status = "Fail",
                        ErrorMessage = ex.Message,
                    };
                };
        }
    }
}

/*
 * API - Application Program Interface
 * Взаємодія програми зі своїми застосунками
 * 
 * 
 *                 Program    ------------API ------- Open(other programs)
 *                 /       \
 *                /    API  \
 * Application                Application
 * (web site)                 (mobile app)
 * 
 * Difference with controllers:
 *                  MVC                 API
 * Адресація      /Ctrl/action         /api/Ctrl
 * Вибір дії        за action        за методом запиту    
 * Повернення     IActRes (View)      object, що перетворюється до JSON автоматично 
   Вага              більша               менша
 */
