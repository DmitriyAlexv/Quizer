using Quizer.Common;
using Quizer.QuizAggregate.Enums;

namespace Quizer.QuizAggregate.Entities;

/// <summary>
/// Вопрос квиза.
/// </summary>
public class Question : Entity
{
    private readonly List<Answer> _answers = [];

    public string Text { get; private set; }

    public QuestionType Type { get; private set; }

    public int Order { get; private set; }

    public int Points { get; private set; }

    public IReadOnlyCollection<Answer> Answers => _answers.AsReadOnly();

    private Question() : this(string.Empty, 0, 0, 0)
    {
    }

    /// <summary>
    /// Вопрос квиза.
    /// </summary>
    public Question(string text, QuestionType type, int order, int points)
    {
        Text = text;
        Type = type;
        Order = order;
        Points = points;
    }

    public void Update(string text, QuestionType type, int order, int points)
    {
        Text = text;
        Type = type;
        Order = order;
        Points = points;
    }

    public Answer AddAnswer(string text, bool isCorrect)
    {
        var answer = new Answer(text, isCorrect);
        _answers.Add(answer);
        return answer;
    }

    public Answer GetAnswer(Guid answerId)
    {
        return _answers.FirstOrDefault(a => a.Id == answerId)
               ?? throw new InvalidOperationException($"Ответ с id {answerId} не найден.");
    }
    
    public void RemoveAnswer(Guid answerId)
    {
        var answer = GetAnswer(answerId);
        _answers.Remove(answer);
    }

    public void UpdateAnswer(Guid answerId, string text, bool isCorrect)
    {
        var answer = GetAnswer(answerId);
        answer.Update(text, isCorrect);
    }
}
