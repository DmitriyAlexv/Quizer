using MediatR;
using Quizer.QuizAggregate;
using Quizer.QuizAggregate.Entities;

namespace Quizer.UseCases.Queries.GetQuestion;

public class GetQuestionQueryHandler(IQuizRepository quizRepository) : IRequestHandler<GetQuestionQuery, Question?>
{
    public async Task<Question?> Handle(GetQuestionQuery request, CancellationToken cancellationToken)
    {
        return await quizRepository.GetQuestionAsync(request.QuizId, request.QuestionId, cancellationToken);
    }
}
