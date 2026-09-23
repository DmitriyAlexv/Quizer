using MediatR;
using Quizer.QuizAggregate;

namespace Quizer.UseCases.Queries.GetQuiz;

public record GetQuizQuery(Guid Id) : IRequest<Quiz?>;
