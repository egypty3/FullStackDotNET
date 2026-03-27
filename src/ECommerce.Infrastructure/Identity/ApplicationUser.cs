using Microsoft.AspNetCore.Identity;

namespace ECommerce.Infrastructure.Identity;

/// <summary>
/// Extends the default ASP.NET Core <see cref="IdentityUser"/> with additional profile properties
/// required by the ECommerce application (first name, last name, and account creation timestamp).
/// This class is mapped to the ASP.NET Identity users table via <see cref="IdentityDbContext{TUser}"/>.
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>Gets or sets the user's first (given) name.</summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>Gets or sets the user's last (family) name.</summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>Gets or sets the UTC timestamp when the user account was created. Defaults to <see cref="DateTime.UtcNow"/>.</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
