using ASP_421.Data.Configuration;
using ASP_421.Data.Entities;
using Microsoft.EntityFrameworkCore;
namespace ASP_421.Data
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<Entities.User> Users { get; set; }
        public DbSet<Entities.UserAccess> UserAccesses { get; set; }
        public DbSet<Entities.UserRole> UserRoles { get; set; }

        public DbSet<Entities.Product> Products { get; set; }
        public DbSet<Entities.ProductGroup> ProductGroups { get; set; }
        public DbSet<ASP_421.Data.Entities.Request> Requests { get; set; } = null!;
      
        public DbSet<Entities.Cart> Carts { get; set; }
        public DbSet<Entities.CartItem> CartItems { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Entities.UserAccess>()
            //    .HasIndex(ua => ua.Login)
            //    .IsUnique();

            //modelBuilder.Entity<Entities.UserAccess>()
            //    .HasOne(ua => ua.User)
            //    .WithMany(u => u.Accesses);

            //modelBuilder.Entity<Entities.UserAccess>()
            //   .HasOne(ua => ua.Role)
            //   .WithMany()
            //   .HasForeignKey(ua=>ua.RoleId);

            modelBuilder.ApplyConfiguration(new UserAccessConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new ASP_421.Data.Configuration.RequestConfiguration());
            modelBuilder.Entity<ASP_421.Data.Entities.User>()
                .HasQueryFilter(u => u.DeletedAt == null);
            modelBuilder.Entity<Entities.Product>()
                .HasIndex(p => p.Slug)
                .IsUnique();
            modelBuilder.Entity<Entities.Product>()
                .HasOne(p => p.Group)
                .WithMany(g => g.Products)
                .HasForeignKey(p => p.GroupId);
            modelBuilder.Entity<Entities.ProductGroup>()
                .HasIndex(p => p.Slug)
                .IsUnique();

            modelBuilder.Entity<ProductGroup>(builder =>
            {
                builder.Property(x => x.Slug)
                .HasMaxLength(64)
                .IsRequired();
                builder.HasIndex(x => x.Slug).IsUnique();
            });

            modelBuilder.Entity<Entities.Cart>()
                .HasMany(c => c.CartItems)
                .WithOne(ci => ci.Cart);

            modelBuilder.Entity<Entities.Cart>()
                .HasOne(c => c.User)
                .WithMany();

            modelBuilder.Entity<Entities.CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany();
        }
    }
}
