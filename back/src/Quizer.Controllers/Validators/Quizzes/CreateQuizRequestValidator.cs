using FluentValidation;
using Quizer.Controllers.Contracts.Quizzes.V1;

namespace Quizer.Controllers.Validators.Quizzes;

/// <summary>
/// Валидатор запроса на создание квиза.
/// </summary>
public class CreateQuizRequestValidator : AbstractValidator<CreateQuizRequest>
{
    public CreateQuizRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Название квиза не может быть пустым.")
            .MaximumLength(200)
            .WithMessage("Название квиза не должно превышать 200 символов.");

        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .WithMessage("Описание квиза не должно превышать 2000 символов.");
    }
}
