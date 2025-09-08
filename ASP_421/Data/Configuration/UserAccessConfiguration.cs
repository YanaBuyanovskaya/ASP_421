using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace ASP_421.Data.Configuration
{
    public class UserAccessConfiguration :
        IEntityTypeConfiguration<Entities.UserAccess>
        {


        public void Configure(EntityTypeBuilder<Entities.UserAccess> builder)
        {
            builder
           .HasIndex(ua => ua.Login)
           .IsUnique();

            builder.HasOne(ua => ua.User)
                .WithMany(u => u.Accesses)
                .HasForeignKey(ua => ua.UserId);

            builder
               .HasOne(ua => ua.Role)
               .WithMany()
               .HasForeignKey(ua => ua.RoleId);

            builder.HasData(new Entities.UserAccess
            {
                Id = Guid.Parse("BBBF2F46-AF0A-437C-8082-BAC4AE83FE94"),
                UserId = Guid.Parse("53759101-7DE4-4E04-833A-884752290FA0"),
                RoleId = "Admin",
                Login = "Admin",
                Salt = "B81D9191-7040-41A3-BFBD-6AF1FFB4A266",
                Dk = "45A97232E9EE2E8EAE3F", //password = Admin
            });
        }
    }
};

