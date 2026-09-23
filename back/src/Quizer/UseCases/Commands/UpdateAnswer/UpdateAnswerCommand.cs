using MediatR;

namespace Quizer.UseCases.Commands.UpdateAnswer;

public record UpdateAnswerCommand(
    Guid QuizId,
    Guid QuestionId,
    Guid AnswerId,
    string Text,
    bool IsCorrect) : IRequest;
