using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quizer.QuizAggregate.Entities;

namespace Quizer.Infrastructure.Data.Quiz.Configurations;

public class AttemptAnswerConfiguration : IEntityTypeConfiguration<AttemptAnswer>
{
    public void Configure(EntityTypeBuilder<AttemptAnswer> builder)
    {
        builder.ToTable("attempt_answers");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.QuestionId)
            .IsRequired();

        builder.Property(a => a.TextAnswer)
            .HasMaxLength(2000);

        builder.Property(a => a.IsCorrect)
            .IsRequired();

        builder.PrimitiveCollection(a => a.SelectedAnswerIds)
            .IsRequired();

        builder.HasIndex("AttemptId");
    }
}
