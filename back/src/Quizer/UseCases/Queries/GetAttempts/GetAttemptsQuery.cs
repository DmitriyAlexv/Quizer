using MediatR;
using Quizer.Abstractions.Pagination;
using Quizer.QuizAggregate.Entities;

namespace Quizer.UseCases.Queries.GetAttempts;

public record GetAttemptsQuery(Guid QuizId, int Page, int PageSize) : IRequest<PagedResult<Attempt>>;
