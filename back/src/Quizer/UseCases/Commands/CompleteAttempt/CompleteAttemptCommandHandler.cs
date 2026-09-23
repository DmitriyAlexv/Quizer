using MediatR;
using Quizer.Abstractions.Data;
using Quizer.Common;
using Quizer.QuizAggregate;
using Quizer.QuizAggregate.Entities;

namespace Quizer.UseCases.Commands.CompleteAttempt;

public class CompleteAttemptCommandHandler(
    IQuizRepository quizRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CompleteAttemptCommand>
{
    public async Task Handle(CompleteAttemptCommand request, CancellationToken cancellationToken)
    {
        var quiz = await quizRepository.GetByIdAsync(request.QuizId, cancellationToken)
                   ?? throw new InvalidOperationException($"Квиз с id {request.QuizId} не найден.");

        var attempt = quiz.GetAttempt(request.AttemptId);
        EnsureAttemptOwner(attempt, request.UserId);

        quiz.CompleteAttempt(request.AttemptId);
        quizRepository.Update(quiz);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static void EnsureAttemptOwner(Attempt attempt, Guid userId)
    {
        if (attempt.UserId != userId)
        {
            throw new ForbiddenException("Только владелец попытки может завершить её.");
        }
    }
}
