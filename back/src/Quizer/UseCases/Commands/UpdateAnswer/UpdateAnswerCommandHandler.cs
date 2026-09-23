using MediatR;
using Quizer.Abstractions.Data;
using Quizer.Common;
using Quizer.QuizAggregate;

namespace Quizer.UseCases.Commands.UpdateAnswer;

public class UpdateAnswerCommandHandler(
    IQuizRepository quizRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateAnswerCommand>
{
    public async Task Handle(UpdateAnswerCommand request, CancellationToken cancellationToken)
    {
        var quiz = await quizRepository.GetByIdAsync(request.QuizId, cancellationToken)
                   ?? throw new InvalidOperationException($"Квиз с id {request.QuizId} не найден.");

        EnsureOwner(quiz, request.OwnerId);

        quiz.UpdateAnswer(request.QuestionId, request.AnswerId, request.Text, request.IsCorrect);
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
