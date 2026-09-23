using MediatR;
using Quizer.QuizAggregate;
using Quizer.QuizAggregate.Entities;

namespace Quizer.UseCases.Queries.GetAttempt;

public class GetAttemptQueryHandler(IQuizRepository quizRepository) : IRequestHandler<GetAttemptQuery, Attempt?>
{
    public async Task<Attempt?> Handle(GetAttemptQuery request, CancellationToken cancellationToken)
    {
        return await quizRepository.GetAttemptAsync(request.QuizId, request.AttemptId, cancellationToken);
    }
}
