using MediatR;
using Quizer.QuizAggregate;
using Quizer.QuizAggregate.Entities;

namespace Quizer.UseCases.Queries.GetAttemptResult;

public class GetAttemptResultQueryHandler(IQuizRepository quizRepository)
    : IRequestHandler<GetAttemptResultQuery, AttemptResultDto?>
{
    public async Task<AttemptResultDto?> Handle(GetAttemptResultQuery request, CancellationToken cancellationToken)
    {
        var quiz = await quizRepository.GetByIdAsync(request.QuizId, cancellationToken);
        if (quiz is null)
        {
            return null;
        }

        var attempt = quiz.Attempts.FirstOrDefault(a => a.Id == request.AttemptId);
        if (attempt is null)
        {
            return null;
        }

        return BuildResult(quiz, attempt);
    }

    private static AttemptResultDto BuildResult(Quiz quiz, Attempt attempt)
    {
        var totalPoints = quiz.Questions.Sum(q => q.Points);
        var earnedPoints = 0;

        var questionResults = new List<QuestionResultDto>();
        foreach (var question in quiz.Questions.OrderBy(q => q.Order))
        {
            var answer = attempt.Answers.FirstOrDefault(a => a.QuestionId == question.Id);
            var isCorrect = answer?.IsCorrect ?? false;
            if (isCorrect)
            {
                earnedPoints += question.Points;
            }

            questionResults.Add(new QuestionResultDto
            {
                QuestionId = question.Id,
                Text = question.Text,
                Type = question.Type,
                Points = question.Points,
                IsCorrect = isCorrect,
                TextAnswer = answer?.TextAnswer,
                SelectedAnswerIds = answer?.SelectedAnswerIds.ToList() ?? [],
            });
        }

        return new AttemptResultDto
        {
            AttemptId = attempt.Id,
            QuizId = attempt.QuizId,
            UserId = attempt.UserId,
            Status = attempt.Status,
            StartedAt = attempt.StartedAt,
            CompletedAt = attempt.CompletedAt,
            TotalPoints = totalPoints,
            EarnedPoints = earnedPoints,
            Questions = questionResults,
        };
    }
}
