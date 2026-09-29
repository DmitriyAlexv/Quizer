using Quizer.Exceptions;
using Quizer.QuizAggregate.Enums;
using Quizer.UnitTests.Base;

namespace Quizer.UnitTests.QuizAggregate;

public class AttemptTests : TestBase
{
    [Fact]
    public void CreateAttempt_SetsInitialState()
    {
        // Arrange
        var quiz = CreateQuiz().Build();
        quiz.Publish();
        
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
        var quiz = CreateQuiz().Build();
        quiz.Publish();
        
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
        var quiz = CreateQuiz().Build();
        quiz.Publish();
        
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
        var quiz = CreateQuiz().Build();
        
        var question = quiz.AddQuestion("Вопрос", QuestionType.Open, 1, 10);
        question.AddAnswer("Ответ", true);
        
        quiz.Publish();

        var attempt = quiz.CreateAttempt(Guid.NewGuid());
        attempt.Complete();

        // Act
        // Assert
        Assert.Throws<ConflictException>(() => attempt.AnswerQuestion(question, "Ответ", null));
    }

    [Fact]
    public void AnswerQuestion_OpenAnswer_Correct_ReturnsCorrect()
    {
        // Arrange
        var quiz = CreateQuiz().Build();
        
        var question = quiz.AddQuestion("Столица Франции?", QuestionType.Open, 1, 10);
        question.AddAnswer("Париж", true);
        
        quiz.Publish();

        var attempt = quiz.CreateAttempt(Guid.NewGuid());

        // Act
        var answer = attempt.AnswerQuestion(question, "париж", null);

        // Assert
        Assert.True(answer.IsCorrect);
    }

    [Fact]
    public void AnswerQuestion_OpenAnswer_Incorrect_ReturnsIncorrect()
    {
        // Arrange
        var quiz = CreateQuiz().Build();
        
        var question = quiz.AddQuestion("Столица Франции?", QuestionType.Open, 1, 10);
        question.AddAnswer("Париж", true);
        
        quiz.Publish();

        var attempt = quiz.CreateAttempt(Guid.NewGuid());

        // Act
        var answer = attempt.AnswerQuestion(question, "Лондон", null);

        // Assert
        Assert.False(answer.IsCorrect);
    }

    [Fact]
    public void AnswerQuestion_OpenAnswer_Empty_ReturnsIncorrect()
    {
        // Arrange
        var quiz = CreateQuiz().Build();
        
        var question = quiz.AddQuestion("Столица Франции?", QuestionType.Open, 1, 10);
        question.AddAnswer("Париж", true);
        
        quiz.Publish();

        var attempt = quiz.CreateAttempt(Guid.NewGuid());

        // Act
        var answer = attempt.AnswerQuestion(question, "", null);

        // Assert
        Assert.False(answer.IsCorrect);
    }

    [Fact]
    public void AnswerQuestion_SingleChoice_Correct_ReturnsCorrect()
    {
        // Arrange
        var quiz = CreateQuiz().Build();
        
        var question = quiz.AddQuestion("Выберите столицу", QuestionType.SingleChoice, 1, 10);
        var correctAnswer = question.AddAnswer("Париж", true);
        question.AddAnswer("Лондон", false);
        
        quiz.Publish();

        var attempt = quiz.CreateAttempt(Guid.NewGuid());

        // Act
        var answer = attempt.AnswerQuestion(question, null, new[] { correctAnswer.Id });

        // Assert
        Assert.True(answer.IsCorrect);
    }

    [Fact]
    public void AnswerQuestion_SingleChoice_Incorrect_ReturnsIncorrect()
    {
        // Arrange
        var quiz = CreateQuiz().Build();
        
        var question = quiz.AddQuestion("Выберите столицу", QuestionType.SingleChoice, 1, 10);
        question.AddAnswer("Париж", true);
        var wrongAnswer = question.AddAnswer("Лондон", false);
        
        quiz.Publish();

        var attempt = quiz.CreateAttempt(Guid.NewGuid());

        // Act
        var answer = attempt.AnswerQuestion(question, null, new[] { wrongAnswer.Id });

        // Assert
        Assert.False(answer.IsCorrect);
    }

    [Fact]
    public void AnswerQuestion_SingleChoice_MultipleSelected_ReturnsIncorrect()
    {
        // Arrange
        var quiz = CreateQuiz().Build();
        
        var question = quiz.AddQuestion("Выберите столицу", QuestionType.SingleChoice, 1, 10);
        var correctAnswer = question.AddAnswer("Париж", true);
        var wrongAnswer = question.AddAnswer("Лондон", false);
        
        quiz.Publish();

        var attempt = quiz.CreateAttempt(Guid.NewGuid());

        // Act
        var answer = attempt.AnswerQuestion(question, null, new[] { correctAnswer.Id, wrongAnswer.Id });

        // Assert
        Assert.False(answer.IsCorrect);
    }

    [Fact]
    public void AnswerQuestion_MultipleChoice_Correct_ReturnsCorrect()
    {
        // Arrange
        var quiz = CreateQuiz().Build();
        
        var question = quiz.AddQuestion("Выберите столицы", QuestionType.MultipleChoice, 1, 10);
        var parisAnswer = question.AddAnswer("Париж", true);
        var londonAnswer = question.AddAnswer("Лондон", true);
        question.AddAnswer("Берлин", false);
        
        quiz.Publish();

        var attempt = quiz.CreateAttempt(Guid.NewGuid());

        // Act
        var answer = attempt.AnswerQuestion(question, null, new[] { parisAnswer.Id, londonAnswer.Id });

        // Assert
        Assert.True(answer.IsCorrect);
    }

    [Fact]
    public void AnswerQuestion_MultipleChoice_Partial_ReturnsIncorrect()
    {
        // Arrange
        var quiz = CreateQuiz().Build();
        
        var question = quiz.AddQuestion("Выберите столицы", QuestionType.MultipleChoice, 1, 10);
        var parisAnswer = question.AddAnswer("Париж", true);
        question.AddAnswer("Лондон", true);
        question.AddAnswer("Берлин", false);
        
        quiz.Publish();

        var attempt = quiz.CreateAttempt(Guid.NewGuid());

        // Act
        var answer = attempt.AnswerQuestion(question, null, new[] { parisAnswer.Id });

        // Assert
        Assert.False(answer.IsCorrect);
    }

    [Fact]
    public void AnswerQuestion_ReplacesExistingAnswer()
    {
        // Arrange
        var quiz = CreateQuiz().Build();
        
        var question = quiz.AddQuestion("Столица Франции?", QuestionType.Open, 1, 10);
        question.AddAnswer("Париж", true);
        
        quiz.Publish();

        var attempt = quiz.CreateAttempt(Guid.NewGuid());

        // Act
        attempt.AnswerQuestion(question, "Лондон", null);
        var answer = attempt.AnswerQuestion(question, "Париж", null);

        // Assert
        Assert.Single(attempt.Answers);
        Assert.True(answer.IsCorrect);
    }
}