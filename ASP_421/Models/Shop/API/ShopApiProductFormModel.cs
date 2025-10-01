using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using ASP_421.Data;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;


namespace ASP_421.Models.Shop.API
{
    public class ShopApiProductFormModel
        : IValidatableObject
    {
        [FromForm(Name = "product-name")]
        [Required(ErrorMessage = "Вкажіть назву товару")]
        [StringLength(120, ErrorMessage = "Назва не довше за 120 символів")]
        public String Name { get; set; } = null!;

        [FromForm(Name = "product-description")]
        [StringLength(4000, ErrorMessage = "Опис товару не довший за 4000 символів")]
        public String? Description { get; set; } = null!;

        [FromForm(Name = "product-slug")]
        [StringLength(160, ErrorMessage = "Slug не довший за 160 символів")]
        public String? Slug { get; set; } = null!;

        [FromForm(Name = "product-image")]
        public IFormFile? Image { get; set; } = null!;

        [FromForm(Name = "product-group-id")]
        [Required(ErrorMessage = "Оберіть групу")]
        public Guid? GroupId { get; set; } = null!;

        [FromForm(Name = "product-price")]
        [Required(ErrorMessage = "Вкажіть ціну")]
        [Range(typeof(decimal), "0.01", "1000000", ErrorMessage = "Ціна має бути > 0")]
        public decimal Price { get; set; }

        [FromForm(Name = "product-stock")]
        [Range(1, 1000000, ErrorMessage = "Кількість не може бути від’ємною")]
        public int Stock { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext ctx)
        {
            if(GroupId is null || GroupId == Guid.Empty)
                yield return new ValidationResult("Оберіть валідну групу", new[] { nameof(GroupId) });
            
            if(Price<=0)
                yield return new ValidationResult("Ціна має бути > 0", new[] { nameof(Price) });

            if(Stock<0)
                yield return new ValidationResult("Кількість не може бути від’ємною", new[] { nameof(Stock) });

            if(Image!=null)
            {
                if(Image.Length>5*1024*1024)
                    yield return new ValidationResult("Зображення не повинно перевищувати 5MB", new[] { nameof(Image) });
                var allowed = new[] { "image/jpeg", "image/png", "image/jpg" };
                if (System.Array.IndexOf(allowed, Image.ContentType) < 0)
                    yield return new ValidationResult("Дозволені формати: jpg, png, jpeg ", new[] { nameof(Image) });
            }
            yield break;
        }

    }
}
