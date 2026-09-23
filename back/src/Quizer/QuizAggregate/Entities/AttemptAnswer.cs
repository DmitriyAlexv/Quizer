using Quizer.Common;

namespace Quizer.QuizAggregate.Entities;

/// <summary>
/// Ответ пользователя на вопрос в рамках попытки прохождения квиза.
/// </summary>
public class AttemptAnswer : Entity
{
    public Guid QuestionId { get; private set; }

    /// <summary>
    /// Текстовый ответ для вопроса с открытым ответом.
    /// </summary>
    public string? TextAnswer { get; private set; }

    /// <summary>
    /// Выбранные варианты ответа для вопросов с выбором.
    /// </summary>
    private readonly List<Guid> _selectedAnswerIds = [];

    public IReadOnlyCollection<Guid> SelectedAnswerIds => _selectedAnswerIds.AsReadOnly();

    /// <summary>
    /// Является ли ответ правильным.
    /// </summary>
    public bool IsCorrect { get; private set; }

    private AttemptAnswer()
    {
    }

    public AttemptAnswer(Guid questionId, string? textAnswer, IEnumerable<Guid>? selectedAnswerIds, bool isCorrect)
    {
        QuestionId = questionId;
        TextAnswer = textAnswer;
        IsCorrect = isCorrect;

        if (selectedAnswerIds is not null)
        {
            _selectedAnswerIds.AddRange(selectedAnswerIds);
        }
    }
}
