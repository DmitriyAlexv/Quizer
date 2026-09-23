using Quizer.QuizAggregate;

namespace Quizer.UnitTests.Base;

/// <summary>
/// Базовый класс для всех тестов.
/// </summary>
public abstract class TestBase
{
    protected static readonly Guid OwnerId = Guid.NewGuid();

    protected static QuizBuilder CreateQuiz() => 
        QuizBuilder.Create()
            .WithOwner(OwnerId);

    protected static Quiz CreateDraftQuiz(string title = "Тестовый квиз", string description = "Описание")
        => CreateQuiz()
            .WithTitle(title)
            .WithDescription(description)
            .Build();
}
