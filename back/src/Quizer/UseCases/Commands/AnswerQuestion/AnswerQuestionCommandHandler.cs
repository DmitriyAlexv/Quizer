using MediatR;
using Quizer.Abstractions.Data;
using Quizer.QuizAggregate;
using Quizer.QuizAggregate.Entities;
using Quizer.QuizAggregate.Enums;

namespace Quizer.UseCases.Commands.AnswerQuestion;

public class AnswerQuestionCommandHandler(
    IQuizRepository quizRepository, 
    IUnitOfWork unitOfWork) : IRequestHandler<AnswerQuestionCommand, AttemptAnswer>
{
    public async Task<AttemptAnswer> Handle(AnswerQuestionCommand request, CancellationToken cancellationToken)
    {
        var quiz = await quizRepository.GetByIdAsync(request.QuizId, cancellationToken)
                   ?? throw new InvalidOperationException($"Квиз с id {request.QuizId} не найден.");

        var answer = quiz.AnswerQuestion(request.AttemptId, request.QuestionId, request.TextAnswer, request.SelectedAnswerIds);

        var attempt = quiz.GetAttempt(request.AttemptId);
        if (attempt.Status == AttemptStatus.InProgress &&
            quiz.Questions.All(q => attempt.Answers.Any(a => a.QuestionId == q.Id)))
        {
            quiz.CompleteAttempt(request.AttemptId);
        }

        quizRepository.Update(quiz);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return answer;
    }
}
