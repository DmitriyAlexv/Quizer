using MediatR;
using Quizer.Abstractions.Pagination;
using Quizer.QuizAggregate;

namespace Quizer.UseCases.Queries.GetQuizzes;

public class GetQuizzesQueryHandler(IQuizRepository quizRepository)
    : IRequestHandler<GetQuizzesQuery, PagedResult<Quiz>>
{
    public async Task<PagedResult<Quiz>> Handle(GetQuizzesQuery request, CancellationToken cancellationToken)
    {
        return await quizRepository.GetListAsync(request.Page, request.PageSize, cancellationToken);
    }
}
