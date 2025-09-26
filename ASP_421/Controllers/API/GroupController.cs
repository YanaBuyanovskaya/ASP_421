using ASP_421.Data;
using ASP_421.Models.Shop.API;
using ASP_421.Services.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public object CreateGroup(ShopApiGroupFormModel formModel)
        {

            //валідація моделі - перевірка полів на правильність
            //у т.ч. унікальність Slug


            try
            {
                _dataContext.ProductGroups.Add(new()
                {
                    Id = Guid.NewGuid(),
                    Name = formModel.Name,
                    Description = formModel.Description,
                    Slug = formModel.Slug,
                    ImageUrl = _storageService.Save(formModel.Image)
                });
                _dataContext.SaveChanges();
                return new
                {
                    Status = "Ok"
                };
            }
            catch(Exception ex)
            {
                return new
                {
                    Status = "Fail",
                    ErrorMessage = ex.Message,
                };
            }
            
            
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
