using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ASP_421.Models.Shop.API
{
    public class ShopApiGroupFormModel
    {
        [FromForm(Name = "group-name")]
        [Required, StringLength(80, MinimumLength =2)]
        public String Name { get; set; } = null!;

        [FromForm(Name = "group-description")]
        [StringLength(1000)]

        public String Description { get; set; } = null!;

        [FromForm(Name = "group-slug")]
        [StringLength(120)]
        public String Slug { get; set; } = null!;
        [FromForm(Name="group-parent-id")]
        public Guid? ParentId { get; set; }

        [FromForm(Name = "group-image")]
        public IFormFile? Image { get; set; } = null!;

        public IEnumerable<ValidationResult> Validate(ValidationContext ctx)
        {
            if (ParentId == Guid.Empty)
                yield return new ValidationResult("Некоректний ідентифікатор батьківської групи", new[] { nameof(ParentId) });

            if (Image is { Length: > 0 })
            {
                if (Image.Length > 5 * 1024 * 1024)
                    yield return new ValidationResult("Зображення ≤ 5MB", new[] { nameof(Image) });

                var ok = new[] { "image/jpeg", "image/png", "image/jpg" };
                if (Array.IndexOf(ok, Image.ContentType) < 0)
                    yield return new ValidationResult("Дозволені формати: jpg, png, jpeg", new[] { nameof(Image) });
            }
            
            }
        }
    }

