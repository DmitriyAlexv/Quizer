using FluentValidation;
using Quizer.Controllers.Contracts.Users.V1;

namespace Quizer.Controllers.Validators.Users;

/// <summary>
/// Валидатор запроса на авторизацию пользователя.
/// </summary>
public class AuthorizeRequestValidator : AbstractValidator<AuthorizeRequest>
{
    public AuthorizeRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email не может быть пустым.")
            .EmailAddress()
            .WithMessage("Некорректный формат email.")
            .MaximumLength(256)
            .WithMessage("Email не должен превышать 256 символов.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Пароль не может быть пустым.")
            .MaximumLength(100)
            .WithMessage("Пароль не должен превышать 100 символов.");
    }
}
