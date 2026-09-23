using MediatR;
using Quizer.QuizAggregate.Enums;

namespace Quizer.UseCases.Commands.UpdateQuestion;

public record UpdateQuestionCommand(
    Guid QuizId,
    Guid QuestionId,
    string Text,
    QuestionType Type,
    int Order,
    int Points) : IRequest;
