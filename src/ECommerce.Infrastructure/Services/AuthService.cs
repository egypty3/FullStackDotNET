using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ECommerce.Infrastructure.Services;

/// <summary>
/// Concrete implementation of <see cref="IAuthService"/> that handles user registration and login
/// using ASP.NET Core Identity for credential management and issues JWT bearer tokens for
/// authenticated sessions.
/// <para>
/// JWT configuration (key, issuer, audience) is read from <c>appsettings.json</c> under the
/// <c>Jwt</c> section. Tokens are signed with HMAC-SHA256 and expire after 2 hours.
/// </para>
/// </summary>
public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of <see cref="AuthService"/>.
    /// </summary>
    /// <param name="userManager">The ASP.NET Core Identity user manager for creating and validating users.</param>
    /// <param name="configuration">Application configuration used to retrieve JWT signing settings.</param>
    public AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    /// <summary>
    /// Registers a new user with the supplied credentials. Returns a failure result if the email
    /// is already taken or the password does not meet the configured complexity requirements.
    /// On success, a JWT token is generated and returned inside the <see cref="AuthResult"/>.
    /// </summary>
    /// <param name="email">The email address (also used as the username).</param>
    /// <param name="password">The plaintext password to hash and store.</param>
    /// <param name="firstName">The user's first name.</param>
    /// <param name="lastName">The user's last name.</param>
    /// <returns>An <see cref="AuthResult"/> containing success status and either a JWT token or an error message.</returns>
    public async Task<AuthResult> RegisterAsync(string email, string password, string firstName, string lastName)
    {
        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser is not null)
            return new AuthResult(false, Error: "A user with this email already exists.");

        var user = new ApplicationUser
        {
            Email = email,
            UserName = email,
            FirstName = firstName,
            LastName = lastName
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            return new AuthResult(false, Error: string.Join("; ", result.Errors.Select(e => e.Description)));

        var token = GenerateJwtToken(user);
        return new AuthResult(true, Token: token);
    }

    /// <summary>
    /// Authenticates a user with the given email and password. Returns a failure result with a
    /// generic error message (to avoid user enumeration) if either the email is not found or
    /// the password is incorrect. On success, a JWT token is issued.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="password">The plaintext password to verify.</param>
    /// <returns>An <see cref="AuthResult"/> containing success status and either a JWT token or an error message.</returns>
    public async Task<AuthResult> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            return new AuthResult(false, Error: "Invalid email or password.");

        var validPassword = await _userManager.CheckPasswordAsync(user, password);
        if (!validPassword)
            return new AuthResult(false, Error: "Invalid email or password.");

        var token = GenerateJwtToken(user);
        return new AuthResult(true, Token: token);
    }

    /// <summary>
    /// Generates a signed JWT bearer token for the specified user. The token includes the user's
    /// ID (<c>sub</c>), email, a unique token identifier (<c>jti</c>), and custom name claims.
    /// Signed with HMAC-SHA256 using the key from <c>Jwt:Key</c> configuration.
    /// </summary>
    /// <param name="user">The authenticated <see cref="ApplicationUser"/> to create claims for.</param>
    /// <returns>A serialized JWT string.</returns>
    /// <exception cref="InvalidOperationException">Thrown when <c>Jwt:Key</c> is missing from configuration.</exception>
    private string GenerateJwtToken(ApplicationUser user)
    {
        var key = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("JWT key is not configured.");
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var securityKey = new SymmetricSecurityKey(keyBytes);
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("firstName", user.FirstName),
            new Claim("lastName", user.LastName)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
