using ASP_421.Data;
using ASP_421.Models.Shop;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ASP_421.Controllers
{
    public class ShopController(DataAccessor dataAccessor) : Controller
    {
        private readonly DataAccessor _dataAccessor = dataAccessor;
        public IActionResult Index()
        {
            ShopIndexViewModel model = new()
            {
                ProductGroups = _dataAccessor.ProductGroups()
            };
            return View(model);
        }

        public IActionResult Group(String id)
        {
            ViewData["id"] = id;
            return View();
        }
        public IActionResult Admin()
        {
            if(HttpContext.User.Identity?.IsAuthenticated ?? false)
            {
                String? role = HttpContext.User.Claims
                    .FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (role == "Admin")
                {
                    ShopAdminViewModel model = new()
                    { 
                        ProductGroups = _dataAccessor.ProductGroups()
                    };

                    return View(model);
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}


/*
 * Д.З. Реалізувати валідацію моделі форми товару(адмінка),
 *      що передається на додавання до БД.
 *      За відсутності помилок очищувати форму від введених
 *      даних.
 *      
 *      Додати до форми створення нової групи поле з 
 *      введенням батьківської групи(для створення підгруп),
 *      доповнити валідацію моделі групи.
 *      
 *      
 *      ***(додатково) За зразком сайту Амазон додати до
 *      карточки групи(на домашній сторінці) відомості 
 *      про підгрупи(або виводити текстом "підгруп (кількість)"
 *      або формувати картинками з зображень перших підгруп 
 *      в одній картці)
 */
