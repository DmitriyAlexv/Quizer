using MediatR;
using Quizer.Abstractions.Data;
using Quizer.QuizAggregate;

namespace Quizer.UseCases.Commands.CreateQuiz;

public class CreateQuizCommandHandler(
    IQuizRepository quizRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateQuizCommand, Guid>
{
    public async Task<Guid> Handle(CreateQuizCommand request, CancellationToken cancellationToken)
    {
        var quiz = new Quiz(request.Title, request.Description, request.OwnerId);
        await quizRepository.AddAsync(quiz, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return quiz.Id;
    }
}
