using ASP_421.Data.Entities;

namespace ASP_421.Models.Shop
{
    public class ShopProductViewModel
    {
        public String SlugOrId { get; set; } = null!;
        public Product? Product { get; set; }
        public IEnumerable<Product> Associations { get; set; } = [];
    }
}
