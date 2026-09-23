using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quizer.QuizAggregate.Entities;

namespace Quizer.Infrastructure.Data.Quiz.Configurations;

public class AnswerConfiguration : IEntityTypeConfiguration<Answer>
{
    public void Configure(EntityTypeBuilder<Answer> builder)
    {
        builder.ToTable("answers");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Text)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(a => a.IsCorrect)
            .IsRequired();

        builder.HasIndex("QuestionId");
    }
}
