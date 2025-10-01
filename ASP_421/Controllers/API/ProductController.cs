using ASP_421.Data;
using ASP_421.Data.Entities;
using ASP_421.Models.Shop.API;
using ASP_421.Services.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.RegularExpressions;

namespace ASP_421.Controllers.API
{
    [Route("api/product")]
    [ApiController]
    public class ProductController(
        IStorageService storageService,
        ILogger<ProductController> logger,
        DataAccessor dataAccessor) : ControllerBase
    {
        private readonly IStorageService _storageService = storageService;
        private readonly ILogger<ProductController> _logger = logger;
        private readonly DataAccessor _dataAccessor = dataAccessor;

        private static string Slugify(string input)
        {
            var s = input.Trim().ToLowerInvariant();
            var sb = new StringBuilder(s.Length);
            foreach (var ch in s)
            {
                if (char.IsLetterOrDigit(ch)) sb.Append(ch);
                else if (char.IsWhiteSpace(ch) || ch == '-' || ch == '_') sb.Append('-');
            }
            s = Regex.Replace(sb.ToString(), "-+", "-").Trim('-');
            return s;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public IActionResult CreateProduct([FromForm] ShopApiProductFormModel formModel)
        {
            //validation of model
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            if (formModel.GroupId is null || formModel.GroupId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(formModel.GroupId), "Оберіть валідну групу");
                return ValidationProblem(ModelState);
            }

            var slug = string.IsNullOrWhiteSpace(formModel.Slug) ? Slugify(formModel.Name) : Slugify(formModel.Slug!);

            string? imageUrl = null;
            if (formModel.Image is FormFile file && file.Length > 0)
            {
                imageUrl = _storageService.Save(file);
            }

            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = formModel.Name,
                Description = formModel.Description,
                Slug = formModel.Slug,
                GroupId = formModel.GroupId,
                Price = formModel.Price,
                Stock = formModel.Stock,
                ImageUrl = imageUrl
            };

            try
            {
                _dataAccessor.AddProduct(product);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Add product error");
                return Problem(
                    title: "Не вдалося додати товар",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError);
            }
            return Created($"/api/product/{product.Id}", new
            {
                product.Id,
                product.Name,
                product.Price,
                product.Stock,
                product.Slug,
                product.ImageUrl,
                product.GroupId
            });
        }
    }
}
