using ECommerce.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ECommerce.Infrastructure.Identity;

namespace ECommerce.Infrastructure.Data;

/// <summary>
/// The main Entity Framework Core database context for the ECommerce application.
/// Inherits from <see cref="IdentityDbContext{ApplicationUser}"/> to integrate ASP.NET Core Identity
/// tables (users, roles, claims) alongside the domain entity tables.
/// <para>
/// Exposes <see cref="DbSet{TEntity}"/> properties for every aggregate root and entity in the domain,
/// applies all <c>IEntityTypeConfiguration</c> definitions discovered in this assembly via
/// <see cref="OnModelCreating"/>, and automatically stamps <c>UpdatedAt</c> on modified entities
/// during <see cref="SaveChangesAsync"/>.
/// </para>
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    /// <summary>Gets the set of <see cref="Product"/> entities.</summary>
    public DbSet<Product> Products => Set<Product>();

    /// <summary>Gets the set of <see cref="Order"/> entities.</summary>
    public DbSet<Order> Orders => Set<Order>();

    /// <summary>Gets the set of <see cref="OrderItem"/> entities.</summary>
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    /// <summary>Gets the set of <see cref="Category"/> entities.</summary>
    public DbSet<Category> Categories => Set<Category>();

    /// <summary>Gets the set of <see cref="Customer"/> entities.</summary>
    public DbSet<Customer> Customers => Set<Customer>();

    /// <summary>Gets the set of <see cref="Employee"/> entities.</summary>
    public DbSet<Employee> Employees => Set<Employee>();

    /// <summary>Gets the set of <see cref="Shipper"/> entities.</summary>
    public DbSet<Shipper> Shippers => Set<Shipper>();

    /// <summary>Gets the set of <see cref="Shipment"/> entities.</summary>
    public DbSet<Shipment> Shipments => Set<Shipment>();

    /// <summary>Gets the set of <see cref="Payment"/> entities.</summary>
    public DbSet<Payment> Payments => Set<Payment>();

    /// <summary>Gets the set of <see cref="Review"/> entities.</summary>
    public DbSet<Review> Reviews => Set<Review>();

    /// <summary>Gets the set of <see cref="Coupon"/> entities.</summary>
    public DbSet<Coupon> Coupons => Set<Coupon>();

    /// <summary>
    /// Initializes a new instance of <see cref="ApplicationDbContext"/> with the specified options.
    /// </summary>
    /// <param name="options">The database context configuration options (connection string, provider, etc.).</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    /// <summary>
    /// Configures the entity model by calling the base Identity configuration and then applying
    /// all <see cref="IEntityTypeConfiguration{TEntity}"/> classes found in this assembly
    /// (e.g., <c>CategoryConfiguration</c>, <c>ProductConfiguration</c>, etc.).
    /// </summary>
    /// <param name="modelBuilder">The model builder provided by EF Core.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    /// <summary>
    /// Persists all pending changes to the database. Before saving, iterates over tracked
    /// <see cref="Domain.Common.BaseEntity"/> entries in the <c>Modified</c> state and sets their
    /// <c>UpdatedAt</c> timestamp to <see cref="DateTime.UtcNow"/> for automatic audit tracking.
    /// </summary>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>The number of state entries written to the database.</returns>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<Domain.Common.BaseEntity>())
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}
