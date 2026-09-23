using MediatR;
using Quizer.Abstractions.Data;
using Quizer.Common;
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

        var attempt = quiz.GetAttempt(request.AttemptId);
        EnsureAttemptOwner(attempt, request.UserId);

        var answer = quiz.AnswerQuestion(request.AttemptId, request.QuestionId, request.TextAnswer, request.SelectedAnswerIds);

        if (attempt.Status == AttemptStatus.InProgress &&
            quiz.Questions.All(q => attempt.Answers.Any(a => a.QuestionId == q.Id)))
        {
            quiz.CompleteAttempt(request.AttemptId);
        }

        quizRepository.Update(quiz);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return answer;
    }

    private static void EnsureAttemptOwner(Attempt attempt, Guid userId)
    {
        if (attempt.UserId != userId)
        {
            throw new ForbiddenException("Только владелец попытки может отвечать на вопросы.");
        }
    }
}
