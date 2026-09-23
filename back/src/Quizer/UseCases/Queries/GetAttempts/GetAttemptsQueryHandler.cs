using MediatR;
using Quizer.Abstractions.Pagination;
using Quizer.QuizAggregate;
using Quizer.QuizAggregate.Entities;

namespace Quizer.UseCases.Queries.GetAttempts;

public class GetAttemptsQueryHandler(IQuizRepository quizRepository)
    : IRequestHandler<GetAttemptsQuery, PagedResult<Attempt>>
{
    public async Task<PagedResult<Attempt>> Handle(GetAttemptsQuery request, CancellationToken cancellationToken)
    {
        return await quizRepository.GetAttemptsAsync(request.QuizId, request.Page, request.PageSize, cancellationToken);
    }
}
