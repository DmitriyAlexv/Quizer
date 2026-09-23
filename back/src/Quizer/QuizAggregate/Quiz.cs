using Quizer.Common;
using Quizer.QuizAggregate.Entities;
using Quizer.QuizAggregate.Enums;

namespace Quizer.QuizAggregate;

/// <summary>
/// Агрегат квиза.
/// </summary>
public class Quiz : AggregateRoot
{
    private readonly List<Question> _questions = [];
    private readonly List<Attempt> _attempts = [];

    public string Title { get; private set; }

    public string Description { get; private set; }

    public Guid OwnerId { get; private set; }

    public QuizStatus Status { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    public IReadOnlyCollection<Question> Questions => _questions.AsReadOnly();

    public IReadOnlyCollection<Attempt> Attempts => _attempts.AsReadOnly();

    private Quiz()
    {
        Title = string.Empty;
        Description = string.Empty;
    }

    public Quiz(string title, string description, Guid ownerId)
    {
        Title = title;
        Description = description;
        OwnerId = ownerId;
        Status = QuizStatus.Draft;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(string title, string description)
    {
        EnsureNotPublished();
        Title = title;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Publish()
    {
        if (Status == QuizStatus.Published)
        {
            return;
        }

        Status = QuizStatus.Published;
        UpdatedAt = DateTime.UtcNow;
    }

    public Question AddQuestion(string text, QuestionType type, int order, int points)
    {
        EnsureNotPublished();
        var question = new Question(text, type, order, points);
        _questions.Add(question);
        UpdatedAt = DateTime.UtcNow;
        return question;
    }

    public void UpdateQuestion(Guid questionId, string text, QuestionType type, int order, int points)
    {
        EnsureNotPublished();
        var question = GetQuestion(questionId);
        question.Update(text, type, order, points);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveQuestion(Guid questionId)
    {
        EnsureNotPublished();
        var question = GetQuestion(questionId);
        _questions.Remove(question);
        UpdatedAt = DateTime.UtcNow;
    }

    public Answer AddAnswer(Guid questionId, string text, bool isCorrect)
    {
        EnsureNotPublished();
        var question = GetQuestion(questionId);
        var answer = question.AddAnswer(text, isCorrect);
        UpdatedAt = DateTime.UtcNow;
        return answer;
    }

    public void UpdateAnswer(Guid questionId, Guid answerId, string text, bool isCorrect)
    {
        EnsureNotPublished();
        var question = GetQuestion(questionId);
        question.UpdateAnswer(answerId, text, isCorrect);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveAnswer(Guid questionId, Guid answerId)
    {
        EnsureNotPublished();
        var question = GetQuestion(questionId);
        question.RemoveAnswer(answerId);
        UpdatedAt = DateTime.UtcNow;
    }

    public Question GetQuestion(Guid questionId)
    {
        return _questions.FirstOrDefault(q => q.Id == questionId)
               ?? throw new InvalidOperationException($"Вопрос с id {questionId} не найден.");
    }

    /// <summary>
    /// Создать новую попытку прохождения квиза. Одна попытка на пользователя.
    /// </summary>
    public Attempt CreateAttempt(Guid userId)
    {
        if (Status != QuizStatus.Published)
        {
            throw new InvalidOperationException("Нельзя проходить неопубликованный квиз.");
        }

        if (_attempts.Any(a => a.UserId == userId))
        {
            throw new InvalidOperationException("Пользователь уже проходил этот квиз.");
        }

        var attempt = new Attempt(Id, userId);
        _attempts.Add(attempt);
        return attempt;
    }

    public Attempt GetAttempt(Guid attemptId)
    {
        return _attempts.FirstOrDefault(a => a.Id == attemptId)
               ?? throw new InvalidOperationException($"Попытка с id {attemptId} не найдена.");
    }

    public AttemptAnswer AnswerQuestion(Guid attemptId, Guid questionId, string? textAnswer, IEnumerable<Guid>? selectedAnswerIds)
    {
        var attempt = GetAttempt(attemptId);
        var question = GetQuestion(questionId);
        return attempt.AnswerQuestion(question, textAnswer, selectedAnswerIds);
    }

    public void CompleteAttempt(Guid attemptId)
    {
        var attempt = GetAttempt(attemptId);
        attempt.Complete();
    }

    private void EnsureNotPublished()
    {
        if (Status == QuizStatus.Published)
        {
            throw new InvalidOperationException("Нельзя изменять опубликованный квиз.");
        }
    }
}
