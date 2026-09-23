using MediatR;
using Quizer.Abstractions.Data;
using Quizer.QuizAggregate;

namespace Quizer.UseCases.Commands.DeleteQuestion;

public class DeleteQuestionCommandHandler(
    IQuizRepository quizRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteQuestionCommand>
{
    public async Task Handle(DeleteQuestionCommand request, CancellationToken cancellationToken)
    {
        var quiz = await quizRepository.GetByIdAsync(request.QuizId, cancellationToken)
                   ?? throw new InvalidOperationException($"Квиз с id {request.QuizId} не найден.");

        quiz.RemoveQuestion(request.QuestionId);
        quizRepository.Update(quiz);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
