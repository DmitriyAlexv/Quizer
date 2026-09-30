using Quizer.QuizAggregate;

namespace Quizer.UnitTests.Base;

/// <summary>
/// Билдер для создания тестовых квизов.
/// </summary>
public class QuizBuilder
{
    private string _title = "Тестовый квиз";
    private string _description = "Описание";
    private Guid _ownerId = Guid.NewGuid();

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

    public Quiz Build()
    {
        return new Quiz(_title, _description, _ownerId);
    }
}