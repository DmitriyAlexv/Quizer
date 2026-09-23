using MediatR;
using Quizer.Abstractions.Data;
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

        quiz.Update(request.Title, request.Description);
        quizRepository.Update(quiz);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
