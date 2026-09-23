using MediatR;
using Microsoft.AspNetCore.Mvc;
using Quizer.Controllers.Contracts.Users.V1;
using Quizer.UseCases.Commands.AuthorizeUser;
using Quizer.UseCases.Commands.RegisterUser;

namespace Quizer.Controllers.Controllers.V1;

[ApiController]
[Route("api/v1/users")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // POST api/v1/users/register
    [HttpPost("register")]
    public async Task<ActionResult<Guid>> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        var id = await _mediator.Send(
            new RegisterUserCommand(request.Email, request.Password, request.Name),
            cancellationToken);

        return Ok(id);
    }

    // POST api/v1/users/authorize
    [HttpPost("authorize")]
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
