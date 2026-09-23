using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Quizer.Infrastructure.Data.User.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<UserAggregate.User>
{
    public void Configure(EntityTypeBuilder<UserAggregate.User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(200);
    }
}
