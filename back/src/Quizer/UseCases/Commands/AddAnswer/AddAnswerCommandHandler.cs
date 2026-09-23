using MediatR;
using Quizer.Abstractions.Data;
using Quizer.QuizAggregate;
using Quizer.QuizAggregate.Entities;

namespace Quizer.UseCases.Commands.AddAnswer;

public class AddAnswerCommandHandler(
    IQuizRepository quizRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<AddAnswerCommand, Answer>
{
    public async Task<Answer> Handle(AddAnswerCommand request, CancellationToken cancellationToken)
    {
        var quiz = await quizRepository.GetByIdAsync(request.QuizId, cancellationToken)
                   ?? throw new InvalidOperationException($"Квиз с id {request.QuizId} не найден.");

        var answer = quiz.AddAnswer(request.QuestionId, request.Text, request.IsCorrect);
        quizRepository.Update(quiz);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return answer;
    }
}
