using MediatR;
using Quizer.QuizAggregate.Entities;

namespace Quizer.UseCases.Commands.AddAnswer;

public record AddAnswerCommand(
    Guid QuizId,
    Guid QuestionId,
    string Text,
    bool IsCorrect,
    Guid OwnerId) : IRequest<Answer>;
