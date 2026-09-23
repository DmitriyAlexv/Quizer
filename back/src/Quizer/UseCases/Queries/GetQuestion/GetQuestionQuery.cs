using MediatR;
using Quizer.QuizAggregate.Entities;

namespace Quizer.UseCases.Queries.GetQuestion;

public record GetQuestionQuery(Guid QuizId, Guid QuestionId) : IRequest<Question?>;
