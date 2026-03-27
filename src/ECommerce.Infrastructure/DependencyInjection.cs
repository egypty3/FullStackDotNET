using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Interfaces;
using ECommerce.Infrastructure.Data;
using ECommerce.Infrastructure.Identity;
using ECommerce.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Infrastructure;

/// <summary>
/// Provides a single extension method to register all Infrastructure-layer services
/// (database context, ASP.NET Core Identity, and repository/service implementations)
/// into the ASP.NET Core dependency injection container.
/// This follows the Composition Root pattern, keeping registration logic out of <c>Program.cs</c>.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers the Infrastructure layer services with the DI container, including:
    /// <list type="bullet">
    ///   <item><description><see cref="ApplicationDbContext"/> configured with SQL Server using the <c>DefaultConnection</c> connection string.</description></item>
    ///   <item><description>ASP.NET Core Identity with <see cref="ApplicationUser"/> and password/email policies.</description></item>
    ///   <item><description><see cref="IUnitOfWork"/> → <see cref="UnitOfWork"/> (scoped lifetime).</description></item>
    ///   <item><description><see cref="IAuthService"/> → <see cref="AuthService"/> (scoped lifetime).</description></item>
    /// </list>
    /// </summary>
    /// <param name="services">The service collection to add registrations to.</param>
    /// <param name="configuration">The application configuration (provides connection strings and Identity options).</param>
    /// <returns>The same <see cref="IServiceCollection"/> for fluent chaining.</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Register EF Core with SQL Server provider and the DefaultConnection string.
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Configure ASP.NET Core Identity with password complexity and unique-email constraints.
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 8;
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        // Dependency Inversion: register concrete implementations against domain/application abstractions.
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
