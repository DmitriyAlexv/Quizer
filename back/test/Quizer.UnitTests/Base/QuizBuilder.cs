using Quizer.QuizAggregate;
using Quizer.QuizAggregate.Entities;
using Quizer.QuizAggregate.Enums;

namespace Quizer.UnitTests.Base;

/// <summary>
/// Fluent-билдер для создания тестовых квизов.
/// </summary>
public class QuizBuilder
{
    private string _title = "Тестовый квиз";
    private string _description = "Описание";
    private Guid _ownerId = Guid.NewGuid();
    private readonly List<Action<Quiz>> _configurators = [];
    private bool _published;

    private QuizBuilder()
    {
    }

    public static QuizBuilder Create() => new();

    public QuizBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public QuizBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public QuizBuilder WithOwner(Guid ownerId)
    {
        _ownerId = ownerId;
        return this;
    }

    public QuizBuilder WithQuestion(string text, QuestionType type, int order, int points, Action<Question>? configure = null)
    {
        _configurators.Add(quiz =>
        {
            var question = quiz.AddQuestion(text, type, order, points);
            configure?.Invoke(question);
        });
        return this;
    }

    public QuizBuilder Published()
    {
        _published = true;
        return this;
    }

    public Quiz Build()
    {
        var quiz = new Quiz(_title, _description, _ownerId);
        foreach (var configurator in _configurators)
        {
            configurator(quiz);
        }

        if (_published)
        {
            quiz.Publish();
        }

        return quiz;
    }
}
