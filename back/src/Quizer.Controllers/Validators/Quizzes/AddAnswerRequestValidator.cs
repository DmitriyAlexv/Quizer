using FluentValidation;
using Quizer.Controllers.Contracts.Quizzes.V1;

namespace Quizer.Controllers.Validators.Quizzes;

/// <summary>
/// Валидатор запроса на добавление варианта ответа к вопросу.
/// </summary>
public class AddAnswerRequestValidator : AbstractValidator<AddAnswerRequest>
{
    public AddAnswerRequestValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty()
            .WithMessage("Текст ответа не может быть пустым.")
            .MaximumLength(1000)
            .WithMessage("Текст ответа не должен превышать 1000 символов.");
    }
}
