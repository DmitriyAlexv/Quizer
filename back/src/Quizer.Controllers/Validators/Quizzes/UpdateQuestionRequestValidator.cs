using FluentValidation;
using Quizer.Controllers.Contracts.Quizzes.V1;

namespace Quizer.Controllers.Validators.Quizzes;

/// <summary>
/// Валидатор запроса на обновление вопроса квиза.
/// </summary>
public class UpdateQuestionRequestValidator : AbstractValidator<UpdateQuestionRequest>
{
    public UpdateQuestionRequestValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty()
            .WithMessage("Текст вопроса не может быть пустым.")
            .MaximumLength(1000)
            .WithMessage("Текст вопроса не должен превышать 1000 символов.");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Недопустимый тип вопроса.");

        RuleFor(x => x.Order)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Порядковый номер вопроса не может быть отрицательным.");

        RuleFor(x => x.Points)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Количество баллов не может быть отрицательным.");
    }
}
