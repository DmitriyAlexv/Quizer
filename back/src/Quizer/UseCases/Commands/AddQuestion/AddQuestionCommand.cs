using MediatR;
using Quizer.QuizAggregate.Enums;

namespace Quizer.UseCases.Commands.AddQuestion;

public record AddQuestionCommand(
    Guid QuizId,
    string Text,
    QuestionType Type,
    int Order,
    int Points) : IRequest<Guid>;
