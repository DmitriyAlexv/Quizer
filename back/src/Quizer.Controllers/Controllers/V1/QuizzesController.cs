using MediatR;
using Microsoft.AspNetCore.Mvc;
using Quizer.Abstractions.Pagination;
using Quizer.Controllers.Contracts.Common;
using Quizer.Controllers.Contracts.Quizzes.V1;
using Quizer.QuizAggregate;
using Quizer.QuizAggregate.Entities;
using Quizer.UseCases.Commands.AddAnswer;
using Quizer.UseCases.Commands.AddQuestion;
using Quizer.UseCases.Commands.AnswerQuestion;
using Quizer.UseCases.Commands.CompleteAttempt;
using Quizer.UseCases.Commands.CreateAttempt;
using Quizer.UseCases.Commands.CreateQuiz;
using Quizer.UseCases.Commands.DeleteQuestion;
using Quizer.UseCases.Commands.DeleteQuiz;
using Quizer.UseCases.Commands.PublishQuiz;
using Quizer.UseCases.Commands.RemoveAnswer;
using Quizer.UseCases.Commands.UpdateAnswer;
using Quizer.UseCases.Commands.UpdateQuestion;
using Quizer.UseCases.Commands.UpdateQuiz;
using Quizer.UseCases.Queries.GetAttempt;
using Quizer.UseCases.Queries.GetAttemptResult;
using Quizer.UseCases.Queries.GetAttempts;
using Quizer.UseCases.Queries.GetLeaderboard;
using Quizer.UseCases.Queries.GetQuestion;
using Quizer.UseCases.Queries.GetQuestions;
using Quizer.UseCases.Queries.GetQuiz;
using Quizer.UseCases.Queries.GetQuizzes;

namespace Quizer.Controllers.Controllers.V1;

[ApiController]
[Route("api/v1/quizzes")]
public class QuizzesController : ControllerBase
{
    private readonly IMediator _mediator;

    public QuizzesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET api/v1/quizzes
    [HttpGet]
    public async Task<ActionResult<PagedResponse<QuizResponse>>> GetQuizzes(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetQuizzesQuery(page, pageSize), cancellationToken);
        return Ok(MapPaged(result, MapQuiz));
    }

    // GET api/v1/quizzes/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<QuizResponse>> GetQuiz(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetQuizQuery(id), cancellationToken);
        if (result is null)
        {
            return NotFound();
        }

        return Ok(MapQuiz(result));
    }

    // POST api/v1/quizzes
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateQuiz(
        [FromBody] CreateQuizRequest request,
        CancellationToken cancellationToken = default)
    {
        var id = await _mediator.Send(
            new CreateQuizCommand(request.Title, request.Description, request.OwnerId),
            cancellationToken);

        return CreatedAtAction(nameof(GetQuiz), new { id }, id);
    }

    // PUT api/v1/quizzes/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateQuiz(
        Guid id,
        [FromBody] UpdateQuizRequest request,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(new UpdateQuizCommand(id, request.Title, request.Description), cancellationToken);
        return NoContent();
    }

    // DELETE api/v1/quizzes/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteQuiz(Guid id, CancellationToken cancellationToken = default)
    {
        await _mediator.Send(new DeleteQuizCommand(id), cancellationToken);
        return NoContent();
    }

    // PATCH api/v1/quizzes/{id}/publish
    [HttpPatch("{id:guid}/publish")]
    public async Task<IActionResult> PublishQuiz(Guid id, CancellationToken cancellationToken = default)
    {
        await _mediator.Send(new PublishQuizCommand(id), cancellationToken);
        return NoContent();
    }

    // GET api/v1/quizzes/{quizId}/questions
    [HttpGet("{quizId:guid}/questions")]
    public async Task<ActionResult<PagedResponse<QuestionResponse>>> GetQuestions(
        Guid quizId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetQuestionsQuery(quizId, page, pageSize), cancellationToken);
        return Ok(MapPaged(result, MapQuestion));
    }

    // GET api/v1/quizzes/{quizId}/questions/{questionId}
    [HttpGet("{quizId:guid}/questions/{questionId:guid}")]
    public async Task<ActionResult<QuestionResponse>> GetQuestion(
        Guid quizId,
        Guid questionId,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetQuestionQuery(quizId, questionId), cancellationToken);
        if (result is null)
        {
            return NotFound();
        }

        return Ok(MapQuestion(result));
    }

    // POST api/v1/quizzes/{quizId}/questions
    [HttpPost("{quizId:guid}/questions")]
    public async Task<ActionResult<Guid>> AddQuestion(
        Guid quizId,
        [FromBody] AddQuestionRequest request,
        CancellationToken cancellationToken = default)
    {
        var id = await _mediator.Send(
            new AddQuestionCommand(quizId, request.Text, request.Type, request.Order, request.Points),
            cancellationToken);

        return CreatedAtAction(nameof(GetQuestion), new { quizId, questionId = id }, id);
    }

    // PUT api/v1/quizzes/{quizId}/questions/{questionId}
    [HttpPut("{quizId:guid}/questions/{questionId:guid}")]
    public async Task<IActionResult> UpdateQuestion(
        Guid quizId,
        Guid questionId,
        [FromBody] UpdateQuestionRequest request,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(
            new UpdateQuestionCommand(quizId, questionId, request.Text, request.Type, request.Order, request.Points),
            cancellationToken);

        return NoContent();
    }

    // DELETE api/v1/quizzes/{quizId}/questions/{questionId}
    [HttpDelete("{quizId:guid}/questions/{questionId:guid}")]
    public async Task<IActionResult> DeleteQuestion(
        Guid quizId,
        Guid questionId,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(new DeleteQuestionCommand(quizId, questionId), cancellationToken);
        return NoContent();
    }

    // POST api/v1/quizzes/{quizId}/questions/{questionId}/answers
    [HttpPost("{quizId:guid}/questions/{questionId:guid}/answers")]
    public async Task<ActionResult<AnswerResponse>> AddAnswer(
        Guid quizId,
        Guid questionId,
        [FromBody] AddAnswerRequest request,
        CancellationToken cancellationToken = default)
    {
        var answer = await _mediator.Send(
            new AddAnswerCommand(quizId, questionId, request.Text, request.IsCorrect),
            cancellationToken);

        return CreatedAtAction(nameof(GetQuestion), new { quizId, questionId }, MapAnswer(answer));
    }

    // PUT api/v1/quizzes/{quizId}/questions/{questionId}/answers/{answerId}
    [HttpPut("{quizId:guid}/questions/{questionId:guid}/answers/{answerId:guid}")]
    public async Task<IActionResult> UpdateAnswer(
        Guid quizId,
        Guid questionId,
        Guid answerId,
        [FromBody] UpdateAnswerRequest request,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(
            new UpdateAnswerCommand(quizId, questionId, answerId, request.Text, request.IsCorrect),
            cancellationToken);

        return NoContent();
    }

    // DELETE api/v1/quizzes/{quizId}/questions/{questionId}/answers/{answerId}
    [HttpDelete("{quizId:guid}/questions/{questionId:guid}/answers/{answerId:guid}")]
    public async Task<IActionResult> RemoveAnswer(
        Guid quizId,
        Guid questionId,
        Guid answerId,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(new RemoveAnswerCommand(quizId, questionId, answerId), cancellationToken);
        return NoContent();
    }

    // GET api/v1/quizzes/{quizId}/attempts
    [HttpGet("{quizId:guid}/attempts")]
    public async Task<ActionResult<PagedResponse<AttemptResponse>>> GetAttempts(
        Guid quizId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetAttemptsQuery(quizId, page, pageSize), cancellationToken);
        return Ok(MapPaged(result, MapAttempt));
    }

    // GET api/v1/quizzes/{quizId}/attempts/{attemptId}
    [HttpGet("{quizId:guid}/attempts/{attemptId:guid}")]
    public async Task<ActionResult<AttemptResponse>> GetAttempt(
        Guid quizId,
        Guid attemptId,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetAttemptQuery(quizId, attemptId), cancellationToken);
        if (result is null)
        {
            return NotFound();
        }

        return Ok(MapAttempt(result));
    }

    // GET api/v1/quizzes/{quizId}/attempts/{attemptId}/result
    [HttpGet("{quizId:guid}/attempts/{attemptId:guid}/result")]
    public async Task<ActionResult<AttemptResultResponse>> GetAttemptResult(
        Guid quizId,
        Guid attemptId,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetAttemptResultQuery(quizId, attemptId), cancellationToken);
        if (result is null)
        {
            return NotFound();
        }

        return Ok(MapAttemptResult(result));
    }

    // POST api/v1/quizzes/{quizId}/attempts
    [HttpPost("{quizId:guid}/attempts")]
    public async Task<ActionResult<AttemptResponse>> CreateAttempt(
        Guid quizId,
        [FromBody] CreateAttemptRequest request,
        CancellationToken cancellationToken = default)
    {
        var attempt = await _mediator.Send(new CreateAttemptCommand(quizId, request.UserId), cancellationToken);
        return CreatedAtAction(nameof(GetAttempt), new { quizId, attemptId = attempt.Id }, MapAttempt(attempt));
    }

    // POST api/v1/quizzes/{quizId}/attempts/{attemptId}/questions/{questionId}/answer
    [HttpPost("{quizId:guid}/attempts/{attemptId:guid}/questions/{questionId:guid}/answer")]
    public async Task<ActionResult<AttemptAnswerResponse>> AnswerQuestion(
        Guid quizId,
        Guid attemptId,
        Guid questionId,
        [FromBody] AnswerQuestionRequest request,
        CancellationToken cancellationToken = default)
    {
        var answer = await _mediator.Send(
            new AnswerQuestionCommand(quizId, attemptId, questionId, request.TextAnswer, request.SelectedAnswerIds),
            cancellationToken);

        return Ok(MapAttemptAnswer(answer));
    }

    // POST api/v1/quizzes/{quizId}/attempts/{attemptId}/complete
    [HttpPost("{quizId:guid}/attempts/{attemptId:guid}/complete")]
    public async Task<IActionResult> CompleteAttempt(
        Guid quizId,
        Guid attemptId,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(new CompleteAttemptCommand(quizId, attemptId), cancellationToken);
        return NoContent();
    }

    // GET api/v1/quizzes/{id}/leaderboard
    [HttpGet("{id:guid}/leaderboard")]
    public async Task<ActionResult<IReadOnlyCollection<LeaderboardEntryResponse>>> GetLeaderboard(
        Guid id,
        [FromQuery] int limit = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetLeaderboardQuery(id, limit), cancellationToken);
        return Ok(result.Select(e => new LeaderboardEntryResponse(e.UserId, e.UserName, e.Score, e.CompletedAt)).ToList());
    }

    private static PagedResponse<TResponse> MapPaged<TEntity, TResponse>(
        PagedResult<TEntity> paged,
        Func<TEntity, TResponse> mapper)
    {
        return new PagedResponse<TResponse>(
            paged.Items.Select(mapper).ToList(),
            paged.TotalCount,
            paged.Page,
            paged.PageSize,
            paged.TotalPages,
            paged.HasPreviousPage,
            paged.HasNextPage);
    }

    private static QuizResponse MapQuiz(Quiz quiz)
    {
        return new QuizResponse(
            quiz.Id,
            quiz.Title,
            quiz.Description,
            quiz.OwnerId,
            quiz.Status,
            quiz.CreatedAt,
            quiz.UpdatedAt);
    }

    private static QuestionResponse MapQuestion(Question question)
    {
        return new QuestionResponse(
            question.Id,
            question.Text,
            question.Type,
            question.Order,
            question.Points,
            question.Answers.Select(MapAnswer).ToList());
    }

    private static AnswerResponse MapAnswer(Answer answer)
    {
        return new AnswerResponse(answer.Id, answer.Text, answer.IsCorrect);
    }

    private static AttemptResponse MapAttempt(Attempt attempt)
    {
        return new AttemptResponse(
            attempt.Id,
            attempt.QuizId,
            attempt.UserId,
            attempt.Status,
            attempt.StartedAt,
            attempt.CompletedAt,
            attempt.Answers.Select(MapAttemptAnswer).ToList());
    }

    private static AttemptAnswerResponse MapAttemptAnswer(AttemptAnswer answer)
    {
        return new AttemptAnswerResponse(
            answer.Id,
            answer.QuestionId,
            answer.TextAnswer,
            answer.SelectedAnswerIds.ToList(),
            answer.IsCorrect);
    }

    private static AttemptResultResponse MapAttemptResult(AttemptResultDto result)
    {
        return new AttemptResultResponse(
            result.AttemptId,
            result.QuizId,
            result.UserId,
            result.Status,
            result.StartedAt,
            result.CompletedAt,
            result.TotalPoints,
            result.EarnedPoints,
            result.Questions.Select(q => new QuestionResultResponse(
                q.QuestionId,
                q.Text,
                q.Type,
                q.Points,
                q.IsCorrect,
                q.TextAnswer,
                q.SelectedAnswerIds)).ToList());
    }
}
