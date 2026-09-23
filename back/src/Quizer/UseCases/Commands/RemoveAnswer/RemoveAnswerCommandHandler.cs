using MediatR;
using Quizer.Abstractions.Data;
using Quizer.Common;
using Quizer.QuizAggregate;

namespace Quizer.UseCases.Commands.RemoveAnswer;

public class RemoveAnswerCommandHandler(
    IQuizRepository quizRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<RemoveAnswerCommand>
{
    public async Task Handle(RemoveAnswerCommand request, CancellationToken cancellationToken)
    {
        var quiz = await quizRepository.GetByIdAsync(request.QuizId, cancellationToken)
                   ?? throw new InvalidOperationException($"Квиз с id {request.QuizId} не найден.");

        EnsureOwner(quiz, request.OwnerId);

        quiz.RemoveAnswer(request.QuestionId, request.AnswerId);
        quizRepository.Update(quiz);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static void EnsureOwner(Quiz quiz, Guid ownerId)
    {
        if (quiz.OwnerId != ownerId)
        {
            throw new ForbiddenException("Только владелец квиза может изменять его.");
        }
    }
}
