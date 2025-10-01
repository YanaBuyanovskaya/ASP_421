using ASP_421.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ASP_421.Controllers
{
    public class GroupControllerMVC : Controller
    {
        private readonly DataContext _db;
        public GroupControllerMVC(DataContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var items = await _db.ProductGroups
                .Where(g => g.DeletedAt == null)
                .OrderBy(g => g.Name)
                .Select(g => new SelectListItem { Value = g.Id.ToString(), Text = g.Name })
                .ToListAsync();

            ViewBag.ParentGroups = items;
            return View();
        }
       
    }
}
