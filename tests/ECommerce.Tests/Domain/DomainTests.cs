using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using ECommerce.Domain.ValueObjects;
using FluentAssertions;

namespace ECommerce.Tests.Domain;

public class ProductTests
{
    private static readonly Guid TestCategoryId = Guid.NewGuid();

    [Fact]
    public void Create_WithValidData_ShouldCreateProduct()
    {
        var product = Product.Create("Test Product", "Description", 29.99m, 100, "SKU-001", TestCategoryId);

        product.Name.Should().Be("Test Product");
        product.Price.Amount.Should().Be(29.99m);
        product.StockQuantity.Should().Be(100);
        product.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Create_WithNegativePrice_ShouldThrow()
    {
        var act = () => Product.Create("Test", "Desc", -10m, 10, "SKU", TestCategoryId);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ReduceStock_SufficientStock_ShouldReduce()
    {
        var product = Product.Create("Test", "Desc", 10m, 50, "SKU", TestCategoryId);
        product.ReduceStock(30);
        product.StockQuantity.Should().Be(20);
    }

    [Fact]
    public void ReduceStock_InsufficientStock_ShouldThrow()
    {
        var product = Product.Create("Test", "Desc", 10m, 5, "SKU", TestCategoryId);
        var act = () => product.ReduceStock(10);
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveFalse()
    {
        var product = Product.Create("Test", "Desc", 10m, 10, "SKU", TestCategoryId);
        product.Deactivate();
        product.IsActive.Should().BeFalse();
    }
}

public class MoneyTests
{
    [Fact]
    public void Add_SameCurrency_ShouldAddAmounts()
    {
        var money1 = new Money(10m, "USD");
        var money2 = new Money(20m, "USD");
        var result = money1.Add(money2);
        result.Amount.Should().Be(30m);
    }

    [Fact]
    public void Add_DifferentCurrency_ShouldThrow()
    {
        var money1 = new Money(10m, "USD");
        var money2 = new Money(20m, "EUR");
        var act = () => money1.Add(money2);
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Create_NegativeAmount_ShouldThrow()
    {
        var act = () => new Money(-1m);
        act.Should().Throw<ArgumentException>();
    }
}

public class OrderTests
{
    private static readonly Guid TestCategoryId = Guid.NewGuid();

    [Fact]
    public void Create_WithValidData_ShouldCreatePendingOrder()
    {
        var address = new Address("123 Main St", "CityX", "StateY", "12345", "US");
        var order = Order.Create("user-1", "user@test.com", address);

        order.Status.Should().Be(OrderStatus.Pending);
        order.Items.Should().BeEmpty();
    }

    [Fact]
    public void AddItem_ShouldAddAndCalculateTotal()
    {
        var address = new Address("123 Main St", "CityX", "StateY", "12345", "US");
        var order = Order.Create("user-1", "user@test.com", address);
        var product = Product.Create("Widget", "A widget", 25m, 100, "W-001", TestCategoryId);

        order.AddItem(product, 3);

        order.Items.Should().HaveCount(1);
        order.TotalAmount.Amount.Should().Be(75m);
    }

    [Fact]
    public void Confirm_EmptyOrder_ShouldThrow()
    {
        var address = new Address("123 Main St", "CityX", "StateY", "12345", "US");
        var order = Order.Create("user-1", "user@test.com", address);

        var act = () => order.Confirm();
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Cancel_DeliveredOrder_ShouldThrow()
    {
        var address = new Address("123 Main St", "CityX", "StateY", "12345", "US");
        var order = Order.Create("user-1", "user@test.com", address);
        var product = Product.Create("Widget", "A widget", 25m, 100, "W-001", TestCategoryId);
        order.AddItem(product, 1);
        order.Confirm();
        order.MarkAsPaid("tx-123");
        order.Ship();
        order.Deliver();

        var act = () => order.Cancel();
        act.Should().Throw<InvalidOperationException>();
    }
}
