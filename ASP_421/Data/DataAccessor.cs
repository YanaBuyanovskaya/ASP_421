using ASP_421.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ASP_421.Data
{
    public class DataAccessor(DataContext dataContext)
    {
        private readonly DataContext _dataContext = dataContext;

        public void AddProductToCart(String userId, String productId)
        {
            Guid userGuid = Guid.Parse(userId);        //винятки від цих операцій
            Guid productGuid = Guid.Parse(productId);   //передаватимуться до контролера
            User user = _dataContext.Users.Find(userGuid)
                ?? throw new KeyNotFoundException("User not found"); 
            Product product = _dataContext.Products.Find(productGuid)
                ?? throw new KeyNotFoundException("Product not found");
            //перевіряємо чи є у користувача відкритий(активний) кошик
            //якщо ні, то відкриваємо(створюємо) новий
            // якщо так то працюємо з відкритим
            //перевіряємо чи є у кошику позиція (Item) з даним товаром
            //якщо ні, то створємо нову
            //якщо так, то додаємо +1 до цієї позиції
            Cart? activeCart = _dataContext
                .Carts
                .Include(c=>c.CartItems)
                .ThenInclude(ci=>ci.Product)
                .FirstOrDefault(c =>
            c.UserId == userGuid &&
            c.PaidAt == null &&
            c.DeletedAt == null);

            if (activeCart== null)
            {
                activeCart = new()
                { 
                    Id = Guid.NewGuid(),
                    UserId = userGuid,
                    CreatedAt = DateTime.Now,
                };
                _dataContext.Carts.Add(activeCart);
            }
            
            CartItem? cartItem = activeCart
                .CartItems
                .FirstOrDefault(ci=>ci.ProductId== productGuid);
            
            if(cartItem ==null)
            {
                cartItem = new()
                {
                    CartId = activeCart.Id,
                    ProductId = productGuid,
                    Quantity = 1,
                    Product = product,
                };
                _dataContext.CartItems.Add(cartItem);
            }
            else
            {
                cartItem.Quantity += 1;
            }

            //перераховуємо ціну всього кошику, з урахуванням можливих акцій
            CalcCartPrice(activeCart);
            //зберігаємо зміни
            _dataContext.SaveChanges();
        }

        private void CalcCartPrice(Cart cart)
        {
            double price = 0.0;
            foreach(var item in cart.CartItems)
            {
                if(item.DiscountItemId!=null)
                {
                    //тут буде перерахунок з урахуванням акцій

                }
                else
                {
                    item.Price = Convert.ToDouble(item.Product.Price * item.Quantity);
                }
                price += item.Price;
            }
            if(cart.DiscountItemId!=null)
            {
                //тут буде перерахунок ціни кошику з урахуванням акції
            }
            cart.Price = price;
        }
        public ProductGroup? GetProductGroupBySlug(String slug)
        {
            return _dataContext
                .ProductGroups
                .Include(g => g.Products)
                .FirstOrDefault(g => g.Slug == slug && g.DeletedAt == null);
        }

        public Product? GetProductBySlug(String slug)
        {
            return _dataContext
                .Products
                .Include(p => p.Group)
                .ThenInclude(g=>g.Products.OrderBy(p=>Guid.NewGuid()).Take(3))
                .FirstOrDefault
                (p => p.DeletedAt == null &&
                (p.Slug == slug || p.Id.ToString() == slug));
        }


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

        public int GetCartTotalQty(string userId)
        {
            return _dataContext.CartItems
                 .Select(x => (int?)x.Quantity)
                 .Sum() ?? 0;
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
