using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

/// <summary>
/// Управление квизами: создание, редактирование, вопросы, ответы, попытки прохождения и результаты.
/// </summary>
[ApiController]
[Authorize]
[Route("api/v1/quizzes")]
public class QuizzesController : ControllerBase
{
    private readonly IMediator _mediator;

    public QuizzesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Получить список квизов с пагинацией.
    /// </summary>
    /// <param name="page">Номер страницы (начиная с 1).</param>
    /// <param name="pageSize">Количество элементов на странице.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Список квизов с информацией о пагинации.</returns>
    /// <response code="200">Список квизов успешно получен.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<QuizResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResponse<QuizResponse>>> GetQuizzes(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetQuizzesQuery(page, pageSize), cancellationToken);
        return Ok(MapPaged(result, MapQuiz));
    }

    /// <summary>
    /// Получить квиз по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор квиза.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Данные квиза.</returns>
    /// <response code="200">Квиз успешно получен.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="404">Квиз не найден.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(QuizResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<QuizResponse>> GetQuiz(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetQuizQuery(id), cancellationToken);
        if (result is null)
        {
            return NotFound();
        }

        return Ok(MapQuiz(result));
    }

    /// <summary>
    /// Создать новый квиз.
    /// </summary>
    /// <param name="request">Данные для создания квиза.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Идентификатор созданного квиза.</returns>
    /// <response code="201">Квиз успешно создан.</response>
    /// <response code="400">Некорректные данные запроса.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Guid>> CreateQuiz(
        [FromBody] CreateQuizRequest request,
        CancellationToken cancellationToken = default)
    {
        var id = await _mediator.Send(
            new CreateQuizCommand(request.Title, request.Description, GetCurrentUserId()),
            cancellationToken);

        return CreatedAtAction(nameof(GetQuiz), new { id }, id);
    }

    /// <summary>
    /// Обновить существующий квиз.
    /// </summary>
    /// <param name="id">Идентификатор квиза.</param>
    /// <param name="request">Данные для обновления квиза.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Статус 204 No Content при успешном обновлении.</returns>
    /// <response code="204">Квиз успешно обновлён.</response>
    /// <response code="400">Некорректные данные запроса.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="403">Нет прав на изменение квиза.</response>
    /// <response code="404">Квиз не найден.</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateQuiz(
        Guid id,
        [FromBody] UpdateQuizRequest request,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(new UpdateQuizCommand(id, request.Title, request.Description, GetCurrentUserId()), cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Удалить квиз.
    /// </summary>
    /// <param name="id">Идентификатор квиза.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Статус 204 No Content при успешном удалении.</returns>
    /// <response code="204">Квиз успешно удалён.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="403">Нет прав на удаление квиза.</response>
    /// <response code="404">Квиз не найден.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteQuiz(Guid id, CancellationToken cancellationToken = default)
    {
        await _mediator.Send(new DeleteQuizCommand(id, GetCurrentUserId()), cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Опубликовать квиз.
    /// </summary>
    /// <param name="id">Идентификатор квиза.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Статус 204 No Content при успешной публикации.</returns>
    /// <response code="204">Квиз успешно опубликован.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="403">Нет прав на публикацию квиза.</response>
    /// <response code="404">Квиз не найден.</response>
    /// <response code="409">Конфликт состояния квиза.</response>
    [HttpPatch("{id:guid}/publish")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> PublishQuiz(Guid id, CancellationToken cancellationToken = default)
    {
        await _mediator.Send(new PublishQuizCommand(id, GetCurrentUserId()), cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Получить список вопросов квиза с пагинацией.
    /// </summary>
    /// <param name="quizId">Идентификатор квиза.</param>
    /// <param name="page">Номер страницы (начиная с 1).</param>
    /// <param name="pageSize">Количество элементов на странице.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Список вопросов квиза с информацией о пагинации.</returns>
    /// <response code="200">Список вопросов успешно получен.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="404">Квиз не найден.</response>
    [HttpGet("{quizId:guid}/questions")]
    [ProducesResponseType(typeof(PagedResponse<QuestionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PagedResponse<QuestionResponse>>> GetQuestions(
        Guid quizId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetQuestionsQuery(quizId, page, pageSize), cancellationToken);
        return Ok(MapPaged(result, MapQuestion));
    }

    /// <summary>
    /// Получить вопрос квиза по идентификатору.
    /// </summary>
    /// <param name="quizId">Идентификатор квиза.</param>
    /// <param name="questionId">Идентификатор вопроса.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Данные вопроса.</returns>
    /// <response code="200">Вопрос успешно получен.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="404">Вопрос или квиз не найден.</response>
    [HttpGet("{quizId:guid}/questions/{questionId:guid}")]
    [ProducesResponseType(typeof(QuestionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Добавить вопрос в квиз.
    /// </summary>
    /// <param name="quizId">Идентификатор квиза.</param>
    /// <param name="request">Данные для добавления вопроса.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Идентификатор созданного вопроса.</returns>
    /// <response code="201">Вопрос успешно создан.</response>
    /// <response code="400">Некорректные данные запроса.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="403">Нет прав на изменение квиза.</response>
    /// <response code="404">Квиз не найден.</response>
    [HttpPost("{quizId:guid}/questions")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Guid>> AddQuestion(
        Guid quizId,
        [FromBody] AddQuestionRequest request,
        CancellationToken cancellationToken = default)
    {
        var id = await _mediator.Send(
            new AddQuestionCommand(quizId, request.Text, request.Type, request.Order, request.Points, GetCurrentUserId()),
            cancellationToken);

        return CreatedAtAction(nameof(GetQuestion), new { quizId, questionId = id }, id);
    }

    /// <summary>
    /// Обновить вопрос квиза.
    /// </summary>
    /// <param name="quizId">Идентификатор квиза.</param>
    /// <param name="questionId">Идентификатор вопроса.</param>
    /// <param name="request">Данные для обновления вопроса.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Статус 204 No Content при успешном обновлении.</returns>
    /// <response code="204">Вопрос успешно обновлён.</response>
    /// <response code="400">Некорректные данные запроса.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="403">Нет прав на изменение квиза.</response>
    /// <response code="404">Вопрос или квиз не найден.</response>
    [HttpPut("{quizId:guid}/questions/{questionId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateQuestion(
        Guid quizId,
        Guid questionId,
        [FromBody] UpdateQuestionRequest request,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(
            new UpdateQuestionCommand(quizId, questionId, request.Text, request.Type, request.Order, request.Points, GetCurrentUserId()),
            cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Удалить вопрос из квиза.
    /// </summary>
    /// <param name="quizId">Идентификатор квиза.</param>
    /// <param name="questionId">Идентификатор вопроса.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Статус 204 No Content при успешном удалении.</returns>
    /// <response code="204">Вопрос успешно удалён.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="403">Нет прав на изменение квиза.</response>
    /// <response code="404">Вопрос или квиз не найден.</response>
    [HttpDelete("{quizId:guid}/questions/{questionId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteQuestion(
        Guid quizId,
        Guid questionId,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(new DeleteQuestionCommand(quizId, questionId, GetCurrentUserId()), cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Добавить вариант ответа к вопросу.
    /// </summary>
    /// <param name="quizId">Идентификатор квиза.</param>
    /// <param name="questionId">Идентификатор вопроса.</param>
    /// <param name="request">Данные для добавления варианта ответа.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Данные созданного варианта ответа.</returns>
    /// <response code="201">Вариант ответа успешно создан.</response>
    /// <response code="400">Некорректные данные запроса.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="403">Нет прав на изменение квиза.</response>
    /// <response code="404">Вопрос или квиз не найден.</response>
    [HttpPost("{quizId:guid}/questions/{questionId:guid}/answers")]
    [ProducesResponseType(typeof(AnswerResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AnswerResponse>> AddAnswer(
        Guid quizId,
        Guid questionId,
        [FromBody] AddAnswerRequest request,
        CancellationToken cancellationToken = default)
    {
        var answer = await _mediator.Send(
            new AddAnswerCommand(quizId, questionId, request.Text, request.IsCorrect, GetCurrentUserId()),
            cancellationToken);

        return CreatedAtAction(nameof(GetQuestion), new { quizId, questionId }, MapAnswer(answer));
    }

    /// <summary>
    /// Обновить вариант ответа.
    /// </summary>
    /// <param name="quizId">Идентификатор квиза.</param>
    /// <param name="questionId">Идентификатор вопроса.</param>
    /// <param name="answerId">Идентификатор варианта ответа.</param>
    /// <param name="request">Данные для обновления варианта ответа.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Статус 204 No Content при успешном обновлении.</returns>
    /// <response code="204">Вариант ответа успешно обновлён.</response>
    /// <response code="400">Некорректные данные запроса.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="403">Нет прав на изменение квиза.</response>
    /// <response code="404">Вариант ответа, вопрос или квиз не найден.</response>
    [HttpPut("{quizId:guid}/questions/{questionId:guid}/answers/{answerId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAnswer(
        Guid quizId,
        Guid questionId,
        Guid answerId,
        [FromBody] UpdateAnswerRequest request,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(
            new UpdateAnswerCommand(quizId, questionId, answerId, request.Text, request.IsCorrect, GetCurrentUserId()),
            cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Удалить вариант ответа.
    /// </summary>
    /// <param name="quizId">Идентификатор квиза.</param>
    /// <param name="questionId">Идентификатор вопроса.</param>
    /// <param name="answerId">Идентификатор варианта ответа.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Статус 204 No Content при успешном удалении.</returns>
    /// <response code="204">Вариант ответа успешно удалён.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="403">Нет прав на изменение квиза.</response>
    /// <response code="404">Вариант ответа, вопрос или квиз не найден.</response>
    [HttpDelete("{quizId:guid}/questions/{questionId:guid}/answers/{answerId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveAnswer(
        Guid quizId,
        Guid questionId,
        Guid answerId,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(new RemoveAnswerCommand(quizId, questionId, answerId, GetCurrentUserId()), cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Получить список попыток прохождения квиза с пагинацией.
    /// </summary>
    /// <param name="quizId">Идентификатор квиза.</param>
    /// <param name="page">Номер страницы (начиная с 1).</param>
    /// <param name="pageSize">Количество элементов на странице.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Список попыток с информацией о пагинации.</returns>
    /// <response code="200">Список попыток успешно получен.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="404">Квиз не найден.</response>
    [HttpGet("{quizId:guid}/attempts")]
    [ProducesResponseType(typeof(PagedResponse<AttemptResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PagedResponse<AttemptResponse>>> GetAttempts(
        Guid quizId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetAttemptsQuery(quizId, page, pageSize), cancellationToken);
        return Ok(MapPaged(result, MapAttempt));
    }

    /// <summary>
    /// Получить попытку прохождения квиза по идентификатору.
    /// </summary>
    /// <param name="quizId">Идентификатор квиза.</param>
    /// <param name="attemptId">Идентификатор попытки.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Данные попытки.</returns>
    /// <response code="200">Попытка успешно получена.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="404">Попытка или квиз не найден.</response>
    [HttpGet("{quizId:guid}/attempts/{attemptId:guid}")]
    [ProducesResponseType(typeof(AttemptResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Получить результат прохождения попытки квиза.
    /// </summary>
    /// <param name="quizId">Идентификатор квиза.</param>
    /// <param name="attemptId">Идентификатор попытки.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Результат прохождения попытки.</returns>
    /// <response code="200">Результат успешно получен.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="404">Попытка или квиз не найден.</response>
    [HttpGet("{quizId:guid}/attempts/{attemptId:guid}/result")]
    [ProducesResponseType(typeof(AttemptResultResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Начать новую попытку прохождения квиза.
    /// </summary>
    /// <param name="quizId">Идентификатор квиза.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Данные созданной попытки.</returns>
    /// <response code="201">Попытка успешно создана.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="404">Квиз не найден.</response>
    /// <response code="409">Конфликт состояния квиза.</response>
    [HttpPost("{quizId:guid}/attempts")]
    [ProducesResponseType(typeof(AttemptResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AttemptResponse>> CreateAttempt(
        Guid quizId,
        CancellationToken cancellationToken = default)
    {
        var attempt = await _mediator.Send(new CreateAttemptCommand(quizId, GetCurrentUserId()), cancellationToken);
        return CreatedAtAction(nameof(GetAttempt), new { quizId, attemptId = attempt.Id }, MapAttempt(attempt));
    }

    /// <summary>
    /// Ответить на вопрос в рамках попытки.
    /// </summary>
    /// <param name="quizId">Идентификатор квиза.</param>
    /// <param name="attemptId">Идентификатор попытки.</param>
    /// <param name="questionId">Идентификатор вопроса.</param>
    /// <param name="request">Данные ответа на вопрос.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Данные ответа на вопрос.</returns>
    /// <response code="200">Ответ успешно сохранён.</response>
    /// <response code="400">Некорректные данные запроса.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="404">Попытка, вопрос или квиз не найден.</response>
    /// <response code="409">Конфликт состояния попытки.</response>
    [HttpPost("{quizId:guid}/attempts/{attemptId:guid}/questions/{questionId:guid}/answer")]
    [ProducesResponseType(typeof(AttemptAnswerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AttemptAnswerResponse>> AnswerQuestion(
        Guid quizId,
        Guid attemptId,
        Guid questionId,
        [FromBody] AnswerQuestionRequest request,
        CancellationToken cancellationToken = default)
    {
        var answer = await _mediator.Send(
            new AnswerQuestionCommand(quizId, attemptId, questionId, request.TextAnswer, request.SelectedAnswerIds, GetCurrentUserId()),
            cancellationToken);

        return Ok(MapAttemptAnswer(answer));
    }

    /// <summary>
    /// Завершить попытку прохождения квиза.
    /// </summary>
    /// <param name="quizId">Идентификатор квиза.</param>
    /// <param name="attemptId">Идентификатор попытки.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Статус 204 No Content при успешном завершении.</returns>
    /// <response code="204">Попытка успешно завершена.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="404">Попытка или квиз не найден.</response>
    /// <response code="409">Конфликт состояния попытки.</response>
    [HttpPost("{quizId:guid}/attempts/{attemptId:guid}/complete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CompleteAttempt(
        Guid quizId,
        Guid attemptId,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(new CompleteAttemptCommand(quizId, attemptId, GetCurrentUserId()), cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Получить таблицу лидеров по квизу.
    /// </summary>
    /// <param name="id">Идентификатор квиза.</param>
    /// <param name="limit">Максимальное количество записей в таблице лидеров.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Список записей таблицы лидеров.</returns>
    /// <response code="200">Таблица лидеров успешно получена.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="404">Квиз не найден.</response>
    [HttpGet("{id:guid}/leaderboard")]
    [ProducesResponseType(typeof(IReadOnlyCollection<LeaderboardEntryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyCollection<LeaderboardEntryResponse>>> GetLeaderboard(
        Guid id,
        [FromQuery] int limit = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetLeaderboardQuery(id, limit), cancellationToken);
        return Ok(result.Select(e => new LeaderboardEntryResponse(e.UserId, e.UserName, e.Score, e.CompletedAt)).ToList());
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                          ?? throw new UnauthorizedAccessException("Не удалось определить идентификатор пользователя из токена.");

        return Guid.Parse(userIdClaim);
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
