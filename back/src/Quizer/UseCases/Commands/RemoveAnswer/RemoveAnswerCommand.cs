using MediatR;

namespace Quizer.UseCases.Commands.RemoveAnswer;

public record RemoveAnswerCommand(Guid QuizId, Guid QuestionId, Guid AnswerId) : IRequest;
