using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using FluentValidation;
using MediatR;

namespace ECommerce.Application.Features.Auth.Commands;

public record LoginCommand(string Email, string Password) : IRequest<Result<string>>;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<string>>
{
    private readonly IAuthService _authService;

    public LoginCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<Result<string>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(request.Email, request.Password);
        return result.Succeeded
            ? Result<string>.Success(result.Token!)
            : Result<string>.Failure(result.Error!);
    }
}
