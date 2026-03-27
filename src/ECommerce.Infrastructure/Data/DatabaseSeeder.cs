using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using ECommerce.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Infrastructure.Data;

/// <summary>
/// Provides a static method to seed the database with initial reference data (admin user, categories,
/// products, customers, employees, shippers, and coupons). Designed to be called once during
/// application startup to ensure the database has a baseline set of data for development and demos.
/// <para>
/// Each entity group is only seeded if the corresponding table is empty, making the method
/// safe to call on every startup (idempotent).
/// </para>
/// </summary>
public static class DatabaseSeeder
{
    /// <summary>
    /// Applies any pending EF Core migrations and then seeds the database with initial data
    /// when the corresponding tables are empty. Creates its own DI scope so it can be called
    /// directly from <c>Program.cs</c> using the root <see cref="IServiceProvider"/>.
    /// </summary>
    /// <param name="serviceProvider">The root service provider used to create a scoped lifetime.</param>
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // Apply any pending migrations before seeding.
        await context.Database.MigrateAsync();

        // Seed a default admin user if no users exist yet.
        if (!await userManager.Users.AnyAsync())
        {
            var admin = new ApplicationUser
            {
                UserName = "admin@ecommerce.com",
                Email = "admin@ecommerce.com",
                FirstName = "Admin",
                LastName = "User",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(admin, "Admin@123456");
        }

        // Seed default product categories used to classify inventory.
        if (!await context.Categories.AnyAsync())
        {
            var categories = new[]
            {
                Category.Create("Electronics", "Electronic devices and gadgets"),
                Category.Create("Clothing", "Apparel and fashion items"),
                Category.Create("Books", "Physical and digital books"),
                Category.Create("Home & Garden", "Home improvement and gardening"),
                Category.Create("Sports", "Sports equipment and gear"),
                Category.Create("Food", "Food and beverages"),
            };

            context.Categories.AddRange(categories);
            await context.SaveChangesAsync();
        }

        // Seed sample products across multiple categories for demonstration.
        if (!await context.Products.AnyAsync())
        {
            var electronics = await context.Categories.FirstAsync(c => c.Name == "Electronics");
            var sports = await context.Categories.FirstAsync(c => c.Name == "Sports");
            var books = await context.Categories.FirstAsync(c => c.Name == "Books");
            var food = await context.Categories.FirstAsync(c => c.Name == "Food");
            var clothing = await context.Categories.FirstAsync(c => c.Name == "Clothing");

            var products = new[]
            {
                Product.Create("Wireless Headphones", "Premium noise-cancelling Bluetooth headphones", 149.99m, 100, "WH-001", electronics.Id, "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=600&q=80"),
                Product.Create("Running Shoes", "Lightweight running shoes with cushioned sole", 89.99m, 200, "RS-001", sports.Id, "https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=600&q=80"),
                Product.Create("Clean Code Book", "Robert C. Martin's guide to writing clean code", 39.99m, 150, "BK-001", books.Id, "https://images.unsplash.com/photo-1532012197267-da84d127e765?w=600&q=80"),
                Product.Create("Organic Coffee Beans", "Premium Arabica coffee beans - 1kg", 24.99m, 500, "FD-001", food.Id, "https://images.unsplash.com/photo-1559056199-641a0ac8b55e?w=600&q=80"),
                Product.Create("Smart Watch", "Fitness tracking smartwatch with heart rate monitor", 299.99m, 75, "SW-001", electronics.Id, "https://images.unsplash.com/photo-1523275335684-37898b6baf30?w=600&q=80"),
                Product.Create("Cotton T-Shirt", "Comfortable 100% organic cotton t-shirt", 29.99m, 300, "CT-001", clothing.Id, "https://images.unsplash.com/photo-1521572163474-6864f9cf17ab?w=600&q=80"),
            };

            context.Products.AddRange(products);
            await context.SaveChangesAsync();
        }

        // Seed sample customer accounts with shipping addresses.
        if (!await context.Customers.AnyAsync())
        {
            var customers = new[]
            {
                Customer.Create("John", "Doe", "john.doe@email.com", "+1-555-0101",
                    new Domain.ValueObjects.Address("123 Main St", "New York", "NY", "10001", "USA")),
                Customer.Create("Jane", "Smith", "jane.smith@email.com", "+1-555-0102",
                    new Domain.ValueObjects.Address("456 Oak Ave", "Los Angeles", "CA", "90001", "USA")),
                Customer.Create("Bob", "Johnson", "bob.johnson@email.com", "+1-555-0103",
                    new Domain.ValueObjects.Address("789 Pine Rd", "Chicago", "IL", "60601", "USA")),
                Customer.Create("Alice", "Williams", "alice.williams@email.com", "+1-555-0104",
                    new Domain.ValueObjects.Address("321 Elm St", "Houston", "TX", "77001", "USA")),
                Customer.Create("Charlie", "Brown", "charlie.brown@email.com", "+1-555-0105",
                    new Domain.ValueObjects.Address("654 Maple Dr", "Phoenix", "AZ", "85001", "USA")),
            };

            context.Customers.AddRange(customers);
            await context.SaveChangesAsync();
        }

        // Seed employee records across various departments.
        if (!await context.Employees.AnyAsync())
        {
            var employees = new[]
            {
                Employee.Create("Sarah", "Connor", "sarah.connor@shopnow.com", "+1-555-1001",
                    "Engineering", "Senior Developer", new DateTime(2023, 1, 15), 95000m),
                Employee.Create("Michael", "Scott", "michael.scott@shopnow.com", "+1-555-1002",
                    "Sales", "Regional Manager", new DateTime(2022, 6, 1), 85000m),
                Employee.Create("Emily", "Chen", "emily.chen@shopnow.com", "+1-555-1003",
                    "Marketing", "Marketing Lead", new DateTime(2023, 3, 20), 78000m),
                Employee.Create("David", "Wilson", "david.wilson@shopnow.com", "+1-555-1004",
                    "Support", "Support Specialist", new DateTime(2024, 1, 10), 55000m),
                Employee.Create("Lisa", "Anderson", "lisa.anderson@shopnow.com", "+1-555-1005",
                    "Engineering", "QA Engineer", new DateTime(2023, 9, 5), 72000m),
                Employee.Create("James", "Taylor", "james.taylor@shopnow.com", "+1-555-1006",
                    "Operations", "Warehouse Manager", new DateTime(2022, 11, 15), 65000m),
            };

            context.Employees.AddRange(employees);
            await context.SaveChangesAsync();
        }

        // Seed shipping carrier companies used for order fulfillment.
        if (!await context.Shippers.AnyAsync())
        {
            var shippers = new[]
            {
                Shipper.Create("FedEx", "+1-800-463-3339", "support@fedex.com", "Tom Harris", "https://www.fedex.com"),
                Shipper.Create("UPS", "+1-800-742-5877", "support@ups.com", "Mary Johnson", "https://www.ups.com"),
                Shipper.Create("DHL Express", "+1-800-225-5345", "support@dhl.com", "Peter Schmidt", "https://www.dhl.com"),
                Shipper.Create("USPS", "+1-800-275-8777", "support@usps.com", "Karen White", "https://www.usps.com"),
            };

            context.Shippers.AddRange(shippers);
            await context.SaveChangesAsync();
        }

        // Seed promotional discount coupons with various types and expiration dates.
        if (!await context.Coupons.AnyAsync())
        {
            var coupons = new[]
            {
                Coupon.Create("WELCOME10", "10% off your first order", DiscountType.Percentage, 10m, 1000, DateTime.UtcNow.AddMonths(6), 25m),
                Coupon.Create("SAVE20", "$20 off orders over $100", DiscountType.FixedAmount, 20m, 500, DateTime.UtcNow.AddMonths(3), 100m),
                Coupon.Create("SUMMER25", "25% summer discount", DiscountType.Percentage, 25m, 200, DateTime.UtcNow.AddMonths(2), 50m),
                Coupon.Create("FREESHIP", "$15 off shipping", DiscountType.FixedAmount, 15m, 300, DateTime.UtcNow.AddMonths(4)),
            };

            context.Coupons.AddRange(coupons);
            await context.SaveChangesAsync();
        }
    }
}
