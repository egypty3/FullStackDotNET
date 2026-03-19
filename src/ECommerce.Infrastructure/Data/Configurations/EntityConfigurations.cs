using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
        builder.HasIndex(c => c.Name).IsUnique();
        builder.Property(c => c.Description).HasMaxLength(500);
    }
}

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Description).HasMaxLength(2000);
        builder.Property(p => p.SKU).IsRequired().HasMaxLength(50);
        builder.HasIndex(p => p.SKU).IsUnique();

        builder.HasOne(p => p.Category)
            .WithMany()
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.OwnsOne(p => p.Price, price =>
        {
            price.Property(m => m.Amount).HasColumnName("Price").HasPrecision(18, 2);
            price.Property(m => m.Currency).HasColumnName("Currency").HasMaxLength(3);
        });
    }
}

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.CustomerId).IsRequired();
        builder.HasIndex(o => o.CustomerId);

        builder.OwnsOne(o => o.ShippingAddress, addr =>
        {
            addr.Property(a => a.Street).HasColumnName("ShippingStreet").HasMaxLength(200);
            addr.Property(a => a.City).HasColumnName("ShippingCity").HasMaxLength(100);
            addr.Property(a => a.State).HasColumnName("ShippingState").HasMaxLength(100);
            addr.Property(a => a.ZipCode).HasColumnName("ShippingZipCode").HasMaxLength(20);
            addr.Property(a => a.Country).HasColumnName("ShippingCountry").HasMaxLength(100);
        });

        builder.OwnsOne(o => o.TotalAmount, total =>
        {
            total.Property(m => m.Amount).HasColumnName("TotalAmount").HasPrecision(18, 2);
            total.Property(m => m.Currency).HasColumnName("TotalCurrency").HasMaxLength(3);
        });

        builder.HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(Order.Items))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.ProductName).HasMaxLength(200);

        builder.OwnsOne(i => i.UnitPrice, price =>
        {
            price.Property(m => m.Amount).HasColumnName("UnitPrice").HasPrecision(18, 2);
            price.Property(m => m.Currency).HasColumnName("UnitCurrency").HasMaxLength(3);
        });

        builder.Ignore(i => i.Subtotal);
    }
}

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(c => c.LastName).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Email).IsRequired().HasMaxLength(200);
        builder.HasIndex(c => c.Email).IsUnique();
        builder.Property(c => c.Phone).HasMaxLength(20);
        builder.Property(c => c.UserId).HasMaxLength(450);
        builder.HasIndex(c => c.UserId).IsUnique().HasFilter("[UserId] IS NOT NULL");
        builder.Ignore(c => c.FullName);

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

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
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
        builder.Ignore(e => e.FullName);
    }
}

public class ShipperConfiguration : IEntityTypeConfiguration<Shipper>
{
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

public class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
{
    public void Configure(EntityTypeBuilder<Shipment> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.TrackingNumber).IsRequired().HasMaxLength(100);
        builder.HasIndex(s => s.TrackingNumber).IsUnique();
        builder.Property(s => s.ShippingCost).HasPrecision(18, 2);
        builder.Property(s => s.Notes).HasMaxLength(1000);

        builder.HasOne(s => s.Order)
            .WithMany()
            .HasForeignKey(s => s.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Shipper)
            .WithMany()
            .HasForeignKey(s => s.ShipperId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.TransactionId).HasMaxLength(200);
        builder.Property(p => p.FailureReason).HasMaxLength(500);

        builder.HasOne(p => p.Order)
            .WithMany()
            .HasForeignKey(p => p.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.OwnsOne(p => p.Amount, amount =>
        {
            amount.Property(m => m.Amount).HasColumnName("Amount").HasPrecision(18, 2);
            amount.Property(m => m.Currency).HasColumnName("Currency").HasMaxLength(3);
        });
    }
}

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
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

        builder.HasIndex(r => new { r.ProductId, r.CustomerId }).IsUnique();
    }
}

public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
{
    public void Configure(EntityTypeBuilder<Coupon> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Code).IsRequired().HasMaxLength(50);
        builder.HasIndex(c => c.Code).IsUnique();
        builder.Property(c => c.Description).HasMaxLength(500);
        builder.Property(c => c.DiscountValue).HasPrecision(18, 2);
        builder.Property(c => c.MinOrderAmount).HasPrecision(18, 2);
        builder.Ignore(c => c.IsExpired);
        builder.Ignore(c => c.IsFullyRedeemed);
        builder.Ignore(c => c.IsValid);
    }
}
