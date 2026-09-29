using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Quizer.Controllers.Contracts.Users.V1;
using Quizer.UseCases.Commands.AuthorizeUser;
using Quizer.UseCases.Commands.RegisterUser;

namespace Quizer.Controllers.Controllers.V1;

/// <summary>
/// Управление пользователями: регистрация и авторизация.
/// </summary>
[ApiController]
[Route("api/v1/users")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Зарегистрировать нового пользователя.
    /// </summary>
    /// <param name="request">Данные для регистрации пользователя.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Идентификатор созданного пользователя.</returns>
    /// <response code="200">Пользователь успешно зарегистрирован.</response>
    /// <response code="400">Некорректные данные запроса.</response>
    /// <response code="409">Пользователь с таким email уже существует.</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Guid>> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        var id = await _mediator.Send(
            new RegisterUserCommand(request.Email, request.Password, request.Name),
            cancellationToken);

        return Ok(id);
    }

    /// <summary>
    /// Авторизовать пользователя и получить JWT-токен.
    /// </summary>
    /// <param name="request">Данные для авторизации пользователя.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>JWT-токен для доступа к защищённым ресурсам.</returns>
    /// <response code="200">Пользователь успешно авторизован.</response>
    /// <response code="400">Некорректные данные запроса.</response>
    /// <response code="401">Неверный email или пароль.</response>
    [HttpPost("authorize")]
    [ProducesResponseType(typeof(AuthorizeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthorizeResponse>> Authorize(
        [FromBody] AuthorizeRequest request,
        CancellationToken cancellationToken = default)
    {
        var token = await _mediator.Send(
            new AuthorizeUserCommand(request.Email, request.Password),
            cancellationToken);

        return Ok(new AuthorizeResponse(token));
    }
}
