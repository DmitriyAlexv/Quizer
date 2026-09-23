using MediatR;
using Quizer.QuizAggregate.Entities;

namespace Quizer.UseCases.Commands.AnswerQuestion;

public record AnswerQuestionCommand(
    Guid QuizId,
    Guid AttemptId,
    Guid QuestionId,
    string? TextAnswer,
    IEnumerable<Guid>? SelectedAnswerIds) : IRequest<AttemptAnswer>;
