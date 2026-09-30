using FluentValidation;
using Quizer.Controllers.Contracts.Quizzes.V1;

namespace Quizer.Controllers.Validators.Quizzes;

/// <summary>
/// Валидатор запроса на ответ на вопрос квиза.
/// </summary>
public class AnswerQuestionRequestValidator : AbstractValidator<AnswerQuestionRequest>
{
    public AnswerQuestionRequestValidator()
    {
        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.TextAnswer) || (x.SelectedAnswerIds?.Any() ?? false))
            .WithMessage("Необходимо указать текстовый ответ или выбранные варианты ответа.");
    }
}
