using ASP_421.Data;
using ASP_421.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASP_421.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupControllerSl : ControllerBase
    {
        private readonly DataContext _dataContext;
        
        public GroupControllerSl(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet("slug")]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            var frontEndBase = Request.Headers["Origin"].FirstOrDefault()
                ?? $"{Request.Scheme}://{Request.Host}";
            string FE(string path) => $"{frontEndBase.TrimEnd('/')}{path}";
            string ABS(string path) => $"{Request.Scheme}://{Request.Host}{path}";


            var g = await _dataContext.ProductGroups
                .AsNoTracking()
                .Include(x => x.Products)
                .FirstOrDefaultAsync(x => x.Slug == slug);

            if (g == null) return NotFound();

            var group = new ProductGroup
            {
                Id = g.Id,
                ParentId = g.ParentId,
                Name = g.Name,
                Description = g.Description,
                Slug = g.Slug,
                ImageUrl = g.ImageUrl is null ? null :
                       (g.ImageUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                          ? g.ImageUrl
                          : ABS(g.ImageUrl.StartsWith("/") ? g.ImageUrl : $"/Storage/Group/{g.ImageUrl}")),
                Url = FE($"/group/{g.Slug}"),
                Products = g.Products
                        .OrderBy(p => p.Name)
                        .Select(p => new Product
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Price = p.Price,
                            Slug = p.Slug,
                            ImageUrl = p.ImageUrl is null ? null :
                                (p.ImageUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                                    ? p.ImageUrl
                                        : ABS(p.ImageUrl.StartsWith("/") ? p.ImageUrl : $"/Storage/Item/{p.ImageUrl}")),
                            Url = FE($"/product/{p.Slug}")
                        })
                        .ToList()

            };
                

            return Ok(group);
        }
    }
}
