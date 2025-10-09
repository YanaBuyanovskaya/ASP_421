using ASP_421.Data.Entities;

namespace ASP_421.Models.Shop
{
    public class ShopGroupViewModel
    {
        public String Slug { get; set; } = null!;
        public ProductGroup? Group { get; set; }
    }
}
