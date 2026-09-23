using Quizer.Common;
using Quizer.QuizAggregate.Enums;

namespace Quizer.QuizAggregate.Entities;

/// <summary>
/// Попытка прохождения квиза пользователем.
/// </summary>
public class Attempt : Entity
{
    private readonly List<AttemptAnswer> _answers = [];

    public Guid QuizId { get; private set; }

    public Guid UserId { get; private set; }

    public AttemptStatus Status { get; private set; }

    public DateTime StartedAt { get; private set; }

    public DateTime? CompletedAt { get; private set; }

    public IReadOnlyCollection<AttemptAnswer> Answers => _answers.AsReadOnly();

    private Attempt()
    {
    }

    public Attempt(Guid quizId, Guid userId)
    {
        QuizId = quizId;
        UserId = userId;
        Status = AttemptStatus.InProgress;
        StartedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Дать ответ на вопрос квиза.
    /// </summary>
    public AttemptAnswer AnswerQuestion(Question question, string? textAnswer, IEnumerable<Guid>? selectedAnswerIds)
    {
        if (Status != AttemptStatus.InProgress)
        {
            throw new InvalidOperationException("Нельзя отвечать на вопросы завершённой попытки.");
        }

        var isCorrect = EvaluateCorrectness(question, textAnswer, selectedAnswerIds);

        var existing = _answers.FirstOrDefault(a => a.QuestionId == question.Id);
        if (existing is not null)
        {
            _answers.Remove(existing);
        }

        var answer = new AttemptAnswer(question.Id, textAnswer, selectedAnswerIds, isCorrect);
        _answers.Add(answer);
        return answer;
    }

    /// <summary>
    /// Завершить попытку прохождения квиза.
    /// </summary>
    public void Complete()
    {
        if (Status == AttemptStatus.Completed)
        {
            return;
        }

        Status = AttemptStatus.Completed;
        CompletedAt = DateTime.UtcNow;
    }

    private static bool EvaluateCorrectness(Question question, string? textAnswer, IEnumerable<Guid>? selectedAnswerIds)
    {
        return question.Type switch
        {
            QuestionType.Open => EvaluateOpenAnswer(question, textAnswer),
            QuestionType.SingleChoice => EvaluateSingleChoice(question, selectedAnswerIds),
            QuestionType.MultipleChoice => EvaluateMultipleChoice(question, selectedAnswerIds),
            _ => false,
        };
    }

    private static bool EvaluateOpenAnswer(Question question, string? textAnswer)
    {
        if (string.IsNullOrWhiteSpace(textAnswer))
        {
            return false;
        }

        var correctAnswer = question.Answers.FirstOrDefault(a => a.IsCorrect);
        return correctAnswer is not null &&
               string.Equals(correctAnswer.Text.Trim(), textAnswer.Trim(), StringComparison.OrdinalIgnoreCase);
    }

    private static bool EvaluateSingleChoice(Question question, IEnumerable<Guid>? selectedAnswerIds)
    {
        var selected = selectedAnswerIds?.ToList() ?? [];
        if (selected.Count != 1)
        {
            return false;
        }

        var correctAnswer = question.Answers.FirstOrDefault(a => a.IsCorrect);
        return correctAnswer is not null && correctAnswer.Id == selected[0];
    }

    private static bool EvaluateMultipleChoice(Question question, IEnumerable<Guid>? selectedAnswerIds)
    {
        var selected = selectedAnswerIds?.ToList() ?? [];
        var correctIds = question.Answers.Where(a => a.IsCorrect).Select(a => a.Id).ToHashSet();

        return correctIds.Count > 0 &&
               correctIds.SetEquals(selected);
    }
}
