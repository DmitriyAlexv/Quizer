using MediatR;
using Quizer.Abstractions.Data;
using Quizer.Common;
using Quizer.QuizAggregate;

namespace Quizer.UseCases.Commands.AddQuestion;

public class AddQuestionCommandHandler(
    IQuizRepository quizRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<AddQuestionCommand, Guid>
{
    public async Task<Guid> Handle(AddQuestionCommand request, CancellationToken cancellationToken)
    {
        var quiz = await quizRepository.GetByIdAsync(request.QuizId, cancellationToken)
                   ?? throw new InvalidOperationException($"Квиз с id {request.QuizId} не найден.");

        EnsureOwner(quiz, request.OwnerId);

        var question = quiz.AddQuestion(request.Text, request.Type, request.Order, request.Points);
        quizRepository.Update(quiz);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return question.Id;
    }

    private static void EnsureOwner(Quiz quiz, Guid ownerId)
    {
        if (quiz.OwnerId != ownerId)
        {
            throw new ForbiddenException("Только владелец квиза может изменять его.");
        }
    }
}
