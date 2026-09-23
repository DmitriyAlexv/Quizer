using MediatR;
using Quizer.Abstractions.Data;
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

        quiz.RemoveAnswer(request.QuestionId, request.AnswerId);
        quizRepository.Update(quiz);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
