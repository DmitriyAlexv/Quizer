using MediatR;
using Quizer.Abstractions.Data;
using Quizer.Common;
using Quizer.QuizAggregate;

namespace Quizer.UseCases.Commands.UpdateQuiz;

public class UpdateQuizCommandHandler(
    IQuizRepository quizRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateQuizCommand>
{
    public async Task Handle(UpdateQuizCommand request, CancellationToken cancellationToken)
    {
        var quiz = await quizRepository.GetByIdAsync(request.Id, cancellationToken)
                   ?? throw new InvalidOperationException($"Квиз с id {request.Id} не найден.");

        EnsureOwner(quiz, request.OwnerId);

        quiz.Update(request.Title, request.Description);
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
