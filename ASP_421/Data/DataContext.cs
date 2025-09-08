using ASP_421.Data.Configuration;
using Microsoft.EntityFrameworkCore;
namespace ASP_421.Data
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<Entities.User> Users { get; set; }
        public DbSet<Entities.UserAccess> UserAccesses { get; set; }
        public DbSet<Entities.UserRole> UserRoles { get; set; }

        public DbSet<ASP_421.Data.Entities.Request> Requests { get; set; } = null!;
       

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
        }
    }
}
