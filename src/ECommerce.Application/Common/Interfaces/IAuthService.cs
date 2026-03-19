namespace ECommerce.Application.Common.Interfaces;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(string email, string password, string firstName, string lastName);
    Task<AuthResult> LoginAsync(string email, string password);
}

public record AuthResult(bool Succeeded, string? Token = null, string? Error = null);
