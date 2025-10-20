using ASP_421.Data;
using ASP_421.Data.Entities;
using ASP_421.Models.Shop;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ASP_421.Controllers
{
    public class ShopController(DataAccessor dataAccessor,
        DataContext dataContext) : Controller
    {
        private readonly DataAccessor _dataAccessor = dataAccessor;
        private readonly DataContext _dataContext = dataContext;
        public IActionResult Index()
        {
            ShopIndexViewModel model = new()
            {
                ProductGroups = _dataAccessor.ProductGroups()
            };
            return View(model);
        }
        [HttpGet]
        public IActionResult Cart(String? id)
        {

            ShopCartViewModel model = new()
            { 
                Cart = id == null
                ? HttpContext.Items["UserCart"] as Data.Entities.Cart
                : _dataAccessor.GetCart(id),
            };
            return View(model);
        }

        public IActionResult Group(String id)
        {
            ShopGroupViewModel model = new()
            {
                Slug = id,
                Group = _dataAccessor.GetProductGroupBySlug(id)
            };
            return View(model);

        }

        public IActionResult Product(String id)
        {
            var product = _dataAccessor.GetProductBySlug(id);


            var model = new ShopProductViewModel
            {
                SlugOrId = id,
                Product = product,
                Associations = product == null ? [] : product.Group.Products
            };
            if (product == null) return NotFound();

            var cart = HttpContext.Items["UserCart"] as Cart;

            if(cart==null && (HttpContext.User.Identity?.IsAuthenticated ?? false))
            {
                var userId = HttpContext.User.Claims
                    .First(c => c.Type == ClaimTypes.PrimarySid)
                    .Value;

                cart = _dataAccessor.GetActiveCart(userId);
            }

            var ci = cart?.CartItems?.FirstOrDefault(x => x.ProductId == product.Id);
            model.inCart = ci != null;
            model.inCartQuantity = ci?.Quantity;

            return View(model);
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

        [HttpGet("/Shop/Product/{id:guid}")]
        public async Task<IActionResult> Prod(Guid id)
        {
            var product = await _dataContext.Products
                .Include(p => p.Group)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            var sameGroup = await _dataContext.Products
                .Where(p => p.GroupId == product.GroupId && p.Id != id)
                .OrderBy(p => Guid.NewGuid())
                .Select(p => new ProductCard
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl
                })
                .Take(3)
                .ToListAsync();

            var excludeIds = sameGroup.Select(x => x.Id).Append(id).ToHashSet();

            var otherGroups = await _dataContext.Products
                .Where(p=>p.GroupId!=product.GroupId && !excludeIds.Contains(p.Id))
                .OrderBy(p=> Guid.NewGuid())
                .Select(p=>new ProductCard
                {
                    Id = p.Id,
                    Name= p.Name,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl
                })
                .Take(3)
                .ToListAsync();

            var alsoLike = sameGroup.Concat(otherGroups).ToList();

            var need = 6 - alsoLike.Count;
            if (need > 0)
            {

                excludeIds.UnionWith(alsoLike.Select(x => x.Id));

                var fill = await _dataContext.Products
                    .Where(p => !excludeIds.Contains(p.Id))
                    .OrderBy(p => Guid.NewGuid())
                    .Select(p => new ProductCard
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Price = p.Price,
                        ImageUrl = p.ImageUrl
                    })
                    .Take(need)
                    .ToListAsync();

                alsoLike.AddRange(fill);

                var cart = HttpContext.Items["UserCart"] as Cart;
                if(cart==null && (User.Identity?.IsAuthenticated ?? false))
                {
                    var userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.PrimarySid).Value;
                    cart = _dataAccessor.GetActiveCart(userId);
                }

                var ci = cart?.CartItems.FirstOrDefault(x => x.ProductId == product.Id);
            }

                var vm = new ProductDetailsVM
                {
                    Product = product,
                    AlsoLike = alsoLike,
                };

                return View("Product", vm);

            }


        }
    }

    public class ProductCard
    {
        public Guid Id { get; set; }
        public String Name { get; set; } = default!;
        public decimal Price { get; set; }
        public String? ImageUrl { get; set; }
    }

    public class ProductDetailsVM
    {
        public Product Product { get; set; } = default!;
        public bool inCart { get; set; }
        public int? inCartQuantity { get; set; }

    public List<ProductCard> AlsoLike { get; set; } = new();
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
