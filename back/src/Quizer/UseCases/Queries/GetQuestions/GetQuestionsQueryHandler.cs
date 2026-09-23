using MediatR;
using Quizer.Abstractions.Pagination;
using Quizer.QuizAggregate;
using Quizer.QuizAggregate.Entities;

namespace Quizer.UseCases.Queries.GetQuestions;

public class GetQuestionsQueryHandler(IQuizRepository quizRepository)
    : IRequestHandler<GetQuestionsQuery, PagedResult<Question>>
{
    public async Task<PagedResult<Question>> Handle(GetQuestionsQuery request, CancellationToken cancellationToken)
    {
        return await quizRepository.GetQuestionsAsync(request.QuizId, request.Page, request.PageSize, cancellationToken);
    }
}
