using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ASP_421.Data.Configuration
{
    public class RequestConfiguration:
        IEntityTypeConfiguration<Entities.Request>
    {
        public void Configure(EntityTypeBuilder<Entities.Request> builder)
        {
            builder.ToTable("Requests");
            builder.HasKey(b => b.Id);

            builder.Property(b => b.Path)
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(b => b.Login)
                .HasMaxLength(50);

            builder.Property(b => b.Answer)
                .IsRequired();

        }
            }
}
