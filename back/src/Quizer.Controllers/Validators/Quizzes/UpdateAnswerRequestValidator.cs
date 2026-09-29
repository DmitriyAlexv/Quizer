using FluentValidation;
using Quizer.Controllers.Contracts.Quizzes.V1;

namespace Quizer.Controllers.Validators.Quizzes;

/// <summary>
/// Валидатор запроса на обновление варианта ответа.
/// </summary>
public class UpdateAnswerRequestValidator : AbstractValidator<UpdateAnswerRequest>
{
    public UpdateAnswerRequestValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty()
            .WithMessage("Текст ответа не может быть пустым.")
            .MaximumLength(1000)
            .WithMessage("Текст ответа не должен превышать 1000 символов.");
    }
}
