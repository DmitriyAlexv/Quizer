using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Quizer.Infrastructure.Data.Quiz.Configurations;

public class QuizConfiguration : IEntityTypeConfiguration<QuizAggregate.Quiz>
{
    public void Configure(EntityTypeBuilder<QuizAggregate.Quiz> builder)
    {
        builder.ToTable("quizzes");

        builder.HasKey(q => q.Id);

        builder.Property(q => q.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(q => q.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(q => q.OwnerId)
            .IsRequired();

        builder.Property(q => q.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(q => q.CreatedAt)
            .IsRequired();

        builder.Property(q => q.UpdatedAt);

        builder.HasMany(q => q.Questions)
            .WithOne()
            .HasForeignKey("QuizId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(q => q.Attempts)
            .WithOne()
            .HasForeignKey("QuizId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(q => q.OwnerId);
    }
}
