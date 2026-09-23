using Quizer.QuizAggregate.Enums;
using Quizer.UnitTests.Base;

namespace Quizer.UnitTests.QuizAggregate;

public class QuizTests : TestBase
{
    [Fact]
    public void CreateQuiz_SetsInitialState()
    {
        // Arrange
        // Act
        var quiz = CreateDraftQuiz();

        // Assert
        Assert.Equal("Тестовый квиз", quiz.Title);
        Assert.Equal("Описание", quiz.Description);
        Assert.Equal(OwnerId, quiz.OwnerId);
        Assert.Equal(QuizStatus.Draft, quiz.Status);
        Assert.Empty(quiz.Questions);
        Assert.Empty(quiz.Attempts);
    }

    [Fact]
    public void Update_ChangesTitleAndDescription()
    {
        // Arrange
        var quiz = CreateDraftQuiz();

        // Act
        quiz.Update("Новый заголовок", "Новое описание");

        // Assert
        Assert.Equal("Новый заголовок", quiz.Title);
        Assert.Equal("Новое описание", quiz.Description);
    }

    [Fact]
    public void Update_OnPublishedQuiz_Throws()
    {
        // Arrange
        var quiz = CreateDraftQuiz();
        quiz.Publish();

        // Act
        // Assert
        Assert.Throws<InvalidOperationException>(() => quiz.Update("Новый", "Заголовок"));
    }

    [Fact]
    public void Publish_SetsStatusToPublished()
    {
        // Arrange
        var quiz = CreateDraftQuiz();

        // Act
        quiz.Publish();

        // Assert
        Assert.Equal(QuizStatus.Published, quiz.Status);
    }

    [Fact]
    public void Publish_WhenAlreadyPublished_DoesNotThrow()
    {
        // Arrange
        var quiz = CreateDraftQuiz();
        quiz.Publish();

        // Act
        quiz.Publish();

        // Assert
        Assert.Equal(QuizStatus.Published, quiz.Status);
    }

    [Fact]
    public void AddQuestion_AddsQuestionToCollection()
    {
        // Arrange
        var quiz = CreateDraftQuiz();

        // Act
        var question = quiz.AddQuestion("Вопрос 1", QuestionType.Open, 1, 10);

        // Assert
        Assert.Single(quiz.Questions);
        Assert.Equal("Вопрос 1", question.Text);
        Assert.Equal(QuestionType.Open, question.Type);
        Assert.Equal(1, question.Order);
        Assert.Equal(10, question.Points);
    }

    [Fact]
    public void AddQuestion_OnPublishedQuiz_Throws()
    {
        // Arrange
        var quiz = CreateDraftQuiz();
        quiz.Publish();

        // Act
        // Assert
        Assert.Throws<InvalidOperationException>(() => quiz.AddQuestion("Вопрос", QuestionType.Open, 1, 10));
    }

    [Fact]
    public void UpdateQuestion_UpdatesQuestion()
    {
        // Arrange
        var quiz = CreateDraftQuiz();
        var question = quiz.AddQuestion("Вопрос 1", QuestionType.Open, 1, 10);

        // Act
        quiz.UpdateQuestion(question.Id, "Обновлённый вопрос", QuestionType.SingleChoice, 2, 20);

        // Assert
        Assert.Equal("Обновлённый вопрос", question.Text);
        Assert.Equal(QuestionType.SingleChoice, question.Type);
        Assert.Equal(2, question.Order);
        Assert.Equal(20, question.Points);
    }

    [Fact]
    public void RemoveQuestion_RemovesQuestion()
    {
        // Arrange
        var quiz = CreateDraftQuiz();
        var question = quiz.AddQuestion("Вопрос 1", QuestionType.Open, 1, 10);

        // Act
        quiz.RemoveQuestion(question.Id);

        // Assert
        Assert.Empty(quiz.Questions);
    }

    [Fact]
    public void GetQuestion_WhenNotFound_Throws()
    {
        // Arrange
        var quiz = CreateDraftQuiz();

        // Act
        // Assert
        Assert.Throws<InvalidOperationException>(() => quiz.GetQuestion(Guid.NewGuid()));
    }

    [Fact]
    public void AddAnswer_AddsAnswerToQuestion()
    {
        // Arrange
        var quiz = CreateDraftQuiz();
        var question = quiz.AddQuestion("Вопрос 1", QuestionType.SingleChoice, 1, 10);

        // Act
        var answer = quiz.AddAnswer(question.Id, "Вариант 1", true);

        // Assert
        Assert.Single(question.Answers);
        Assert.Equal("Вариант 1", answer.Text);
        Assert.True(answer.IsCorrect);
    }

    [Fact]
    public void CreateAttempt_OnDraftQuiz_Throws()
    {
        // Arrange
        var quiz = CreateDraftQuiz();

        // Act
        // Assert
        Assert.Throws<InvalidOperationException>(() => quiz.CreateAttempt(Guid.NewGuid()));
    }

    [Fact]
    public void CreateAttempt_OnPublishedQuiz_CreatesAttempt()
    {
        // Arrange
        var quiz = CreateDraftQuiz();
        quiz.Publish();
        var userId = Guid.NewGuid();

        // Act
        var attempt = quiz.CreateAttempt(userId);

        // Assert
        Assert.Single(quiz.Attempts);
        Assert.Equal(userId, attempt.UserId);
        Assert.Equal(quiz.Id, attempt.QuizId);
    }

    [Fact]
    public void CreateAttempt_WhenUserAlreadyAttempted_Throws()
    {
        // Arrange
        var quiz = CreateDraftQuiz();
        quiz.Publish();
        var userId = Guid.NewGuid();
        quiz.CreateAttempt(userId);

        // Act
        // Assert
        Assert.Throws<InvalidOperationException>(() => quiz.CreateAttempt(userId));
    }

    [Fact]
    public void GetAttempt_WhenNotFound_Throws()
    {
        // Arrange
        var quiz = CreateDraftQuiz();

        // Act
        // Assert
        Assert.Throws<InvalidOperationException>(() => quiz.GetAttempt(Guid.NewGuid()));
    }
}
