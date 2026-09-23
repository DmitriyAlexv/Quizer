using MediatR;
using Quizer.QuizAggregate;

namespace Quizer.UseCases.Queries.GetQuiz;

public class GetQuizQueryHandler(IQuizRepository quizRepository) : IRequestHandler<GetQuizQuery, Quiz?>
{
    public async Task<Quiz?> Handle(GetQuizQuery request, CancellationToken cancellationToken)
    {
        return await quizRepository.GetByIdAsync(request.Id, cancellationToken);
    }
}
