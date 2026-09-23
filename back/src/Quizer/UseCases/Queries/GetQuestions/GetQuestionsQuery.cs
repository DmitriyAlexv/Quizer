using MediatR;
using Quizer.Abstractions.Pagination;
using Quizer.QuizAggregate.Entities;

namespace Quizer.UseCases.Queries.GetQuestions;

public record GetQuestionsQuery(Guid QuizId, int Page, int PageSize) : IRequest<PagedResult<Question>>;
