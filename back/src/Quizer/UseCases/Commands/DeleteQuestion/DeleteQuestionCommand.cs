using MediatR;

namespace Quizer.UseCases.Commands.DeleteQuestion;

public record DeleteQuestionCommand(Guid QuizId, Guid QuestionId, Guid OwnerId) : IRequest;
