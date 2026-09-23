using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quizer.QuizAggregate.Entities;

namespace Quizer.Infrastructure.Data.Quiz.Configurations;

public class AttemptConfiguration : IEntityTypeConfiguration<Attempt>
{
    public void Configure(EntityTypeBuilder<Attempt> builder)
    {
        builder.ToTable("attempts");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.QuizId)
            .IsRequired();

        builder.Property(a => a.UserId)
            .IsRequired();

        builder.Property(a => a.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(a => a.StartedAt)
            .IsRequired();

        builder.Property(a => a.CompletedAt);

        builder.HasMany(a => a.Answers)
            .WithOne()
            .HasForeignKey("AttemptId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(a => a.QuizId);
        builder.HasIndex(a => a.UserId);
    }
}
