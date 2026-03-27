using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations;

/// <summary>
/// Configures the EF Core entity mapping for the <see cref="Category"/> entity.
/// Defines the primary key, required properties, column constraints, and a unique index on <c>Name</c>.
/// </summary>
public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    /// <summary>
    /// Applies the Category table schema configuration to the <see cref="EntityTypeBuilder{Category}"/>.
    /// </summary>
    /// <param name="builder">The builder used to configure the Category entity type.</param>
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
        builder.HasIndex(c => c.Name).IsUnique();
        builder.Property(c => c.Description).HasMaxLength(500);
    }
}

/// <summary>
/// Configures the EF Core entity mapping for the <see cref="Product"/> entity.
/// Maps the primary key, required columns, a unique SKU index, the relationship to <see cref="Category"/>
/// (with restricted delete), and the owned <c>Price</c> value object (Amount + Currency columns).
/// </summary>
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    /// <summary>
    /// Applies the Product table schema configuration, including the owned <c>Price</c> value object
    /// and the foreign-key relationship to <see cref="Category"/>.
    /// </summary>
    /// <param name="builder">The builder used to configure the Product entity type.</param>
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Description).HasMaxLength(2000);
        builder.Property(p => p.SKU).IsRequired().HasMaxLength(50);
        builder.HasIndex(p => p.SKU).IsUnique();

        // Restrict delete to prevent orphan products when a category is removed.
        builder.HasOne(p => p.Category)
            .WithMany()
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Map the Money value object to flattened Price / Currency columns.
        builder.OwnsOne(p => p.Price, price =>
        {
            price.Property(m => m.Amount).HasColumnName("Price").HasPrecision(18, 2);
            price.Property(m => m.Currency).HasColumnName("Currency").HasMaxLength(3);
        });
    }
}

/// <summary>
/// Configures the EF Core entity mapping for the <see cref="Order"/> entity.
/// Maps the primary key, customer index, the owned <c>ShippingAddress</c> and <c>TotalAmount</c> value objects,
/// and the one-to-many relationship to <see cref="OrderItem"/> (cascading deletes).
/// The Items navigation is configured with field-level access to enforce encapsulation via the backing field.
/// </summary>
public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    /// <summary>
    /// Applies the Order table schema configuration, including owned value objects
    /// (<c>ShippingAddress</c>, <c>TotalAmount</c>) and the Items collection with cascade delete.
    /// </summary>
    /// <param name="builder">The builder used to configure the Order entity type.</param>
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.CustomerId).IsRequired();
        builder.HasIndex(o => o.CustomerId);

        // Map the Address value object to flattened Shipping* columns.
        builder.OwnsOne(o => o.ShippingAddress, addr =>
        {
            addr.Property(a => a.Street).HasColumnName("ShippingStreet").HasMaxLength(200);
            addr.Property(a => a.City).HasColumnName("ShippingCity").HasMaxLength(100);
            addr.Property(a => a.State).HasColumnName("ShippingState").HasMaxLength(100);
            addr.Property(a => a.ZipCode).HasColumnName("ShippingZipCode").HasMaxLength(20);
            addr.Property(a => a.Country).HasColumnName("ShippingCountry").HasMaxLength(100);
        });

        // Map the Money value object to flattened TotalAmount / TotalCurrency columns.
        builder.OwnsOne(o => o.TotalAmount, total =>
        {
            total.Property(m => m.Amount).HasColumnName("TotalAmount").HasPrecision(18, 2);
            total.Property(m => m.Currency).HasColumnName("TotalCurrency").HasMaxLength(3);
        });

        // Order items are cascade-deleted when the parent order is removed.
        builder.HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Use the backing field to enforce DDD encapsulation of the Items collection.
        builder.Metadata.FindNavigation(nameof(Order.Items))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}

/// <summary>
/// Configures the EF Core entity mapping for the <see cref="OrderItem"/> entity.
/// Defines the primary key, the owned <c>UnitPrice</c> value object, and ignores the computed
/// <c>Subtotal</c> property (calculated in memory, not persisted).
/// </summary>
public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    /// <summary>
    /// Applies the OrderItem table schema configuration, including the owned <c>UnitPrice</c> value object.
    /// The <c>Subtotal</c> property is ignored because it is computed at runtime.
    /// </summary>
    /// <param name="builder">The builder used to configure the OrderItem entity type.</param>
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.ProductName).HasMaxLength(200);

        // Map the Money value object to flattened UnitPrice / UnitCurrency columns.
        builder.OwnsOne(i => i.UnitPrice, price =>
        {
            price.Property(m => m.Amount).HasColumnName("UnitPrice").HasPrecision(18, 2);
            price.Property(m => m.Currency).HasColumnName("UnitCurrency").HasMaxLength(3);
        });

        // Subtotal is a derived/computed property; not stored in the database.
        builder.Ignore(i => i.Subtotal);
    }
}

/// <summary>
/// Configures the EF Core entity mapping for the <see cref="Customer"/> entity.
/// Defines required fields, unique email index, an optional unique index on <c>UserId</c>
/// (filtered for non-null values), and the owned <c>ShippingAddress</c> value object.
/// The computed <c>FullName</c> property is excluded from the schema.
/// </summary>
public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    /// <summary>
    /// Applies the Customer table schema configuration, including a filtered unique index on <c>UserId</c>
    /// and the owned <c>ShippingAddress</c> value object.
    /// </summary>
    /// <param name="builder">The builder used to configure the Customer entity type.</param>
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(c => c.LastName).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Email).IsRequired().HasMaxLength(200);
        builder.HasIndex(c => c.Email).IsUnique();
        builder.Property(c => c.Phone).HasMaxLength(20);
        builder.Property(c => c.UserId).HasMaxLength(450);

        // Filtered unique index: only enforced when UserId is not null.
        builder.HasIndex(c => c.UserId).IsUnique().HasFilter("[UserId] IS NOT NULL");

        // FullName is a derived property; not stored in the database.
        builder.Ignore(c => c.FullName);

        // Map the Address value object to flattened Shipping* columns.
        builder.OwnsOne(c => c.ShippingAddress, addr =>
        {
            addr.Property(a => a.Street).HasColumnName("ShippingStreet").HasMaxLength(200);
            addr.Property(a => a.City).HasColumnName("ShippingCity").HasMaxLength(100);
            addr.Property(a => a.State).HasColumnName("ShippingState").HasMaxLength(100);
            addr.Property(a => a.ZipCode).HasColumnName("ShippingZipCode").HasMaxLength(20);
            addr.Property(a => a.Country).HasColumnName("ShippingCountry").HasMaxLength(100);
        });
    }
}

/// <summary>
/// Configures the EF Core entity mapping for the <see cref="Employee"/> entity.
/// Defines required columns, a unique email index, salary precision, and excludes the
/// computed <c>FullName</c> property from persistence.
/// </summary>
public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    /// <summary>
    /// Applies the Employee table schema configuration with salary stored as decimal(18,2).
    /// </summary>
    /// <param name="builder">The builder used to configure the Employee entity type.</param>
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(e => e.LastName).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Email).IsRequired().HasMaxLength(200);
        builder.HasIndex(e => e.Email).IsUnique();
        builder.Property(e => e.Phone).HasMaxLength(20);
        builder.Property(e => e.Department).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Position).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Salary).HasPrecision(18, 2);

        // FullName is a derived property; not stored in the database.
        builder.Ignore(e => e.FullName);
    }
}

/// <summary>
/// Configures the EF Core entity mapping for the <see cref="Shipper"/> entity.
/// Defines required fields, a unique index on <c>CompanyName</c>, and optional contact details.
/// </summary>
public class ShipperConfiguration : IEntityTypeConfiguration<Shipper>
{
    /// <summary>
    /// Applies the Shipper table schema configuration with a unique company name constraint.
    /// </summary>
    /// <param name="builder">The builder used to configure the Shipper entity type.</param>
    public void Configure(EntityTypeBuilder<Shipper> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.CompanyName).IsRequired().HasMaxLength(200);
        builder.HasIndex(s => s.CompanyName).IsUnique();
        builder.Property(s => s.Phone).IsRequired().HasMaxLength(20);
        builder.Property(s => s.Email).HasMaxLength(200);
        builder.Property(s => s.Website).HasMaxLength(500);
        builder.Property(s => s.ContactPerson).HasMaxLength(200);
    }
}

/// <summary>
/// Configures the EF Core entity mapping for the <see cref="Shipment"/> entity.
/// Defines required columns, a unique tracking-number index, shipping cost precision,
/// and foreign-key relationships to <see cref="Order"/> and <see cref="Shipper"/>
/// (both using restricted delete to prevent accidental cascade deletions).
/// </summary>
public class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
{
    /// <summary>
    /// Applies the Shipment table schema configuration, including restricted-delete
    /// foreign keys to both <see cref="Order"/> and <see cref="Shipper"/>.
    /// </summary>
    /// <param name="builder">The builder used to configure the Shipment entity type.</param>
    public void Configure(EntityTypeBuilder<Shipment> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.TrackingNumber).IsRequired().HasMaxLength(100);
        builder.HasIndex(s => s.TrackingNumber).IsUnique();
        builder.Property(s => s.ShippingCost).HasPrecision(18, 2);
        builder.Property(s => s.Notes).HasMaxLength(1000);

        // Restrict delete to prevent removing an order that has active shipments.
        builder.HasOne(s => s.Order)
            .WithMany()
            .HasForeignKey(s => s.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        // Restrict delete to prevent removing a shipper that has associated shipments.
        builder.HasOne(s => s.Shipper)
            .WithMany()
            .HasForeignKey(s => s.ShipperId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

/// <summary>
/// Configures the EF Core entity mapping for the <see cref="Payment"/> entity.
/// Maps the primary key, optional transaction ID and failure reason columns,
/// the foreign-key relationship to <see cref="Order"/> (restricted delete),
/// and the owned <c>Amount</c> value object (Amount + Currency columns).
/// </summary>
public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    /// <summary>
    /// Applies the Payment table schema configuration, including the owned <c>Amount</c> value object
    /// and restricted-delete foreign key to <see cref="Order"/>.
    /// </summary>
    /// <param name="builder">The builder used to configure the Payment entity type.</param>
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.TransactionId).HasMaxLength(200);
        builder.Property(p => p.FailureReason).HasMaxLength(500);

        // Restrict delete to prevent removing an order that has associated payments.
        builder.HasOne(p => p.Order)
            .WithMany()
            .HasForeignKey(p => p.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        // Map the Money value object to flattened Amount / Currency columns.
        builder.OwnsOne(p => p.Amount, amount =>
        {
            amount.Property(m => m.Amount).HasColumnName("Amount").HasPrecision(18, 2);
            amount.Property(m => m.Currency).HasColumnName("Currency").HasMaxLength(3);
        });
    }
}

/// <summary>
/// Configures the EF Core entity mapping for the <see cref="Review"/> entity.
/// Defines required columns, foreign-key relationships to <see cref="Product"/> and <see cref="Customer"/>
/// (both restricted delete), and a composite unique index on (ProductId, CustomerId) to ensure
/// a customer can submit only one review per product.
/// </summary>
public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    /// <summary>
    /// Applies the Review table schema configuration with a composite unique constraint
    /// enforcing one review per customer per product.
    /// </summary>
    /// <param name="builder">The builder used to configure the Review entity type.</param>
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Title).IsRequired().HasMaxLength(200);
        builder.Property(r => r.Comment).HasMaxLength(2000);
        builder.Property(r => r.Rating).IsRequired();

        builder.HasOne(r => r.Product)
            .WithMany()
            .HasForeignKey(r => r.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Customer)
            .WithMany()
            .HasForeignKey(r => r.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Composite unique index: one review per customer per product.
        builder.HasIndex(r => new { r.ProductId, r.CustomerId }).IsUnique();
    }
}

/// <summary>
/// Configures the EF Core entity mapping for the <see cref="Coupon"/> entity.
/// Defines required columns, a unique index on <c>Code</c>, decimal precision for discount
/// and minimum order values, and excludes computed properties (<c>IsExpired</c>,
/// <c>IsFullyRedeemed</c>, <c>IsValid</c>) that are evaluated in memory.
/// </summary>
public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
{
    /// <summary>
    /// Applies the Coupon table schema configuration. Computed validity properties
    /// (<c>IsExpired</c>, <c>IsFullyRedeemed</c>, <c>IsValid</c>) are excluded from the database schema.
    /// </summary>
    /// <param name="builder">The builder used to configure the Coupon entity type.</param>
    public void Configure(EntityTypeBuilder<Coupon> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Code).IsRequired().HasMaxLength(50);
        builder.HasIndex(c => c.Code).IsUnique();
        builder.Property(c => c.Description).HasMaxLength(500);
        builder.Property(c => c.DiscountValue).HasPrecision(18, 2);
        builder.Property(c => c.MinOrderAmount).HasPrecision(18, 2);

        // These are computed/derived properties evaluated at runtime; not persisted.
        builder.Ignore(c => c.IsExpired);
        builder.Ignore(c => c.IsFullyRedeemed);
        builder.Ignore(c => c.IsValid);
    }
}
