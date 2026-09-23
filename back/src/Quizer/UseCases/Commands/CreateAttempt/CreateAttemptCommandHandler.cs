using MediatR;
using Quizer.Abstractions.Data;
using Quizer.QuizAggregate;
using Quizer.QuizAggregate.Entities;

namespace Quizer.UseCases.Commands.CreateAttempt;

public class CreateAttemptCommandHandler(
    IQuizRepository quizRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateAttemptCommand, Attempt>
{
    public async Task<Attempt> Handle(CreateAttemptCommand request, CancellationToken cancellationToken)
    {
        var quiz = await quizRepository.GetByIdAsync(request.QuizId, cancellationToken)
                   ?? throw new InvalidOperationException($"Квиз с id {request.QuizId} не найден.");

        var attempt = quiz.CreateAttempt(request.UserId);
        quizRepository.Update(quiz);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return attempt;
    }
}
