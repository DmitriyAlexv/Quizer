using MediatR;
using Quizer.Abstractions.Data;
using Quizer.QuizAggregate;

namespace Quizer.UseCases.Commands.PublishQuiz;

public class PublishQuizCommandHandler(
    IQuizRepository quizRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<PublishQuizCommand>
{
    public async Task Handle(PublishQuizCommand request, CancellationToken cancellationToken)
    {
        var quiz = await quizRepository.GetByIdAsync(request.Id, cancellationToken)
                   ?? throw new InvalidOperationException($"Квиз с id {request.Id} не найден.");

        quiz.Publish();
        quizRepository.Update(quiz);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
