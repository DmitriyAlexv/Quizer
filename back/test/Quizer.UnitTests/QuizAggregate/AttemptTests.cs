using Quizer.QuizAggregate.Entities;
using Quizer.QuizAggregate.Enums;
using Quizer.UnitTests.Base;

namespace Quizer.UnitTests.QuizAggregate;

public class AttemptTests : TestBase
{
    [Fact]
    public void CreateAttempt_SetsInitialState()
    {
        // Arrange
        var quiz = CreateQuiz().Published().Build();
        var userId = Guid.NewGuid();

        // Act
        var attempt = quiz.CreateAttempt(userId);

        // Assert
        Assert.Equal(AttemptStatus.InProgress, attempt.Status);
        Assert.Equal(userId, attempt.UserId);
        Assert.Equal(quiz.Id, attempt.QuizId);
        Assert.Empty(attempt.Answers);
        Assert.Null(attempt.CompletedAt);
    }

    [Fact]
    public void Complete_SetsStatusToCompleted()
    {
        // Arrange
        var quiz = CreateQuiz().Published().Build();
        var attempt = quiz.CreateAttempt(Guid.NewGuid());

        // Act
        attempt.Complete();

        // Assert
        Assert.Equal(AttemptStatus.Completed, attempt.Status);
        Assert.NotNull(attempt.CompletedAt);
    }

    [Fact]
    public void Complete_WhenAlreadyCompleted_DoesNotThrow()
    {
        // Arrange
        var quiz = CreateQuiz().Published().Build();
        var attempt = quiz.CreateAttempt(Guid.NewGuid());
        attempt.Complete();

        // Act
        attempt.Complete();

        // Assert
        Assert.Equal(AttemptStatus.Completed, attempt.Status);
    }

    [Fact]
    public void AnswerQuestion_OnCompletedAttempt_Throws()
    {
        // Arrange
        Question? question = null;
        var quiz = CreateQuiz()
            .WithQuestion("Вопрос", QuestionType.Open, 1, 10, q =>
            {
                question = q;
                q.AddAnswer("Ответ", true);
            })
            .Published()
            .Build();
        var attempt = quiz.CreateAttempt(Guid.NewGuid());
        attempt.Complete();

        // Act
        // Assert
        Assert.Throws<InvalidOperationException>(() => attempt.AnswerQuestion(question!, "Ответ", null));
    }

    [Fact]
    public void AnswerQuestion_OpenAnswer_Correct_ReturnsCorrect()
    {
        // Arrange
        Question? question = null;
        var quiz = CreateQuiz()
            .WithQuestion("Столица Франции?", QuestionType.Open, 1, 10, q =>
            {
                question = q;
                q.AddAnswer("Париж", true);
            })
            .Published()
            .Build();
        var attempt = quiz.CreateAttempt(Guid.NewGuid());

        // Act
        var answer = attempt.AnswerQuestion(question!, "париж", null);

        // Assert
        Assert.True(answer.IsCorrect);
    }

    [Fact]
    public void AnswerQuestion_OpenAnswer_Incorrect_ReturnsIncorrect()
    {
        // Arrange
        Question? question = null;
        var quiz = CreateQuiz()
            .WithQuestion("Столица Франции?", QuestionType.Open, 1, 10, q =>
            {
                question = q;
                q.AddAnswer("Париж", true);
            })
            .Published()
            .Build();
        var attempt = quiz.CreateAttempt(Guid.NewGuid());

        // Act
        var answer = attempt.AnswerQuestion(question!, "Лондон", null);

        // Assert
        Assert.False(answer.IsCorrect);
    }

    [Fact]
    public void AnswerQuestion_OpenAnswer_Empty_ReturnsIncorrect()
    {
        // Arrange
        Question? question = null;
        var quiz = CreateQuiz()
            .WithQuestion("Столица Франции?", QuestionType.Open, 1, 10, q =>
            {
                question = q;
                q.AddAnswer("Париж", true);
            })
            .Published()
            .Build();
        var attempt = quiz.CreateAttempt(Guid.NewGuid());

        // Act
        var answer = attempt.AnswerQuestion(question!, "", null);

        // Assert
        Assert.False(answer.IsCorrect);
    }

    [Fact]
    public void AnswerQuestion_SingleChoice_Correct_ReturnsCorrect()
    {
        // Arrange
        Question? question = null;
        Guid correctId = default;
        var quiz = CreateQuiz()
            .WithQuestion("Выберите столицу", QuestionType.SingleChoice, 1, 10, q =>
            {
                question = q;
                correctId = q.AddAnswer("Париж", true).Id;
                q.AddAnswer("Лондон", false);
            })
            .Published()
            .Build();
        var attempt = quiz.CreateAttempt(Guid.NewGuid());

        // Act
        var answer = attempt.AnswerQuestion(question!, null, new[] { correctId });

        // Assert
        Assert.True(answer.IsCorrect);
    }

    [Fact]
    public void AnswerQuestion_SingleChoice_Incorrect_ReturnsIncorrect()
    {
        // Arrange
        Question? question = null;
        Guid wrongId = default;
        var quiz = CreateQuiz()
            .WithQuestion("Выберите столицу", QuestionType.SingleChoice, 1, 10, q =>
            {
                question = q;
                q.AddAnswer("Париж", true);
                wrongId = q.AddAnswer("Лондон", false).Id;
            })
            .Published()
            .Build();
        var attempt = quiz.CreateAttempt(Guid.NewGuid());

        // Act
        var answer = attempt.AnswerQuestion(question!, null, new[] { wrongId });

        // Assert
        Assert.False(answer.IsCorrect);
    }

    [Fact]
    public void AnswerQuestion_SingleChoice_MultipleSelected_ReturnsIncorrect()
    {
        // Arrange
        Question? question = null;
        Guid correctId = default;
        Guid wrongId = default;
        var quiz = CreateQuiz()
            .WithQuestion("Выберите столицу", QuestionType.SingleChoice, 1, 10, q =>
            {
                question = q;
                correctId = q.AddAnswer("Париж", true).Id;
                wrongId = q.AddAnswer("Лондон", false).Id;
            })
            .Published()
            .Build();
        var attempt = quiz.CreateAttempt(Guid.NewGuid());

        // Act
        var answer = attempt.AnswerQuestion(question!, null, new[] { correctId, wrongId });

        // Assert
        Assert.False(answer.IsCorrect);
    }

    [Fact]
    public void AnswerQuestion_MultipleChoice_Correct_ReturnsCorrect()
    {
        // Arrange
        Question? question = null;
        Guid parisId = default;
        Guid londonId = default;
        var quiz = CreateQuiz()
            .WithQuestion("Выберите столицы", QuestionType.MultipleChoice, 1, 10, q =>
            {
                question = q;
                parisId = q.AddAnswer("Париж", true).Id;
                londonId = q.AddAnswer("Лондон", true).Id;
                q.AddAnswer("Берлин", false);
            })
            .Published()
            .Build();
        var attempt = quiz.CreateAttempt(Guid.NewGuid());

        // Act
        var answer = attempt.AnswerQuestion(question!, null, new[] { parisId, londonId });

        // Assert
        Assert.True(answer.IsCorrect);
    }

    [Fact]
    public void AnswerQuestion_MultipleChoice_Partial_ReturnsIncorrect()
    {
        // Arrange
        Question? question = null;
        Guid parisId = default;
        var quiz = CreateQuiz()
            .WithQuestion("Выберите столицы", QuestionType.MultipleChoice, 1, 10, q =>
            {
                question = q;
                parisId = q.AddAnswer("Париж", true).Id;
                q.AddAnswer("Лондон", true);
                q.AddAnswer("Берлин", false);
            })
            .Published()
            .Build();
        var attempt = quiz.CreateAttempt(Guid.NewGuid());

        // Act
        var answer = attempt.AnswerQuestion(question!, null, new[] { parisId });

        // Assert
        Assert.False(answer.IsCorrect);
    }

    [Fact]
    public void AnswerQuestion_ReplacesExistingAnswer()
    {
        // Arrange
        Question? question = null;
        var quiz = CreateQuiz()
            .WithQuestion("Столица Франции?", QuestionType.Open, 1, 10, q =>
            {
                question = q;
                q.AddAnswer("Париж", true);
            })
            .Published()
            .Build();
        var attempt = quiz.CreateAttempt(Guid.NewGuid());

        // Act
        attempt.AnswerQuestion(question!, "Лондон", null);
        var answer = attempt.AnswerQuestion(question!, "Париж", null);

        // Assert
        Assert.Single(attempt.Answers);
        Assert.True(answer.IsCorrect);
    }
}
