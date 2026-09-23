using MediatR;
using Quizer.Abstractions.Pagination;
using Quizer.QuizAggregate;

namespace Quizer.UseCases.Queries.GetQuizzes;

public record GetQuizzesQuery(int Page, int PageSize) : IRequest<PagedResult<Quiz>>;
