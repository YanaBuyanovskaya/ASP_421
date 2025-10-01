using ASP_421.Data.Entities;

namespace ASP_421.Data
{
    public class DataAccessor(DataContext dataContext)
    {
        private readonly DataContext _dataContext = dataContext;

        public void AddProduct(Product product)
        {
            if(product.Id == default)
            {
                product.Id = Guid.NewGuid();
            }
            product.DeletedAt = null;
            _dataContext.Products.Add(product);
            _dataContext.SaveChanges();
        }
        public IEnumerable<ProductGroup> ProductGroups()
        {
            return _dataContext
                .ProductGroups
                .Where(g => g.DeletedAt == null)
                .AsEnumerable(); 
        }
    }
}
