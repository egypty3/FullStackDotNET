using ECommerce.Domain.Common;
using ECommerce.Domain.Enums;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Domain.Entities;

public class Order : BaseEntity
{
    public string CustomerId { get; private set; } = string.Empty;
    public string CustomerEmail { get; private set; } = string.Empty;
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;
    public Address ShippingAddress { get; private set; } = null!;
    public Money TotalAmount { get; private set; } = new(0);
    public PaymentStatus PaymentStatus { get; private set; } = PaymentStatus.Pending;
    public string? PaymentTransactionId { get; private set; }
    public DateTime? PaidAt { get; private set; }

    private readonly List<OrderItem> _items = new();
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    private Order() { } // EF Core

    public static Order Create(string customerId, string customerEmail, Address shippingAddress)
    {
        if (string.IsNullOrWhiteSpace(customerId))
            throw new ArgumentException("Customer ID is required.");
        if (string.IsNullOrWhiteSpace(customerEmail))
            throw new ArgumentException("Customer email is required.");

        return new Order
        {
            CustomerId = customerId,
            CustomerEmail = customerEmail,
            ShippingAddress = shippingAddress
        };
    }

    public void AddItem(Product product, int quantity)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Cannot modify a confirmed order.");
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive.");

        var existingItem = _items.FirstOrDefault(i => i.ProductId == product.Id);
        if (existingItem != null)
        {
            existingItem.UpdateQuantity(existingItem.Quantity + quantity);
        }
        else
        {
            _items.Add(OrderItem.Create(Id, product.Id, product.Name, product.Price, quantity));
        }

        RecalculateTotal();
    }

    public void RemoveItem(Guid productId)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Cannot modify a confirmed order.");

        var item = _items.FirstOrDefault(i => i.ProductId == productId)
            ?? throw new InvalidOperationException("Item not found in order.");

        _items.Remove(item);
        RecalculateTotal();
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Only pending orders can be confirmed.");
        if (!_items.Any())
            throw new InvalidOperationException("Cannot confirm an empty order.");

        Status = OrderStatus.Confirmed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsPaid(string transactionId)
    {
        PaymentStatus = PaymentStatus.Completed;
        PaymentTransactionId = transactionId;
        PaidAt = DateTime.UtcNow;
        Status = OrderStatus.Processing;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Ship()
    {
        if (Status != OrderStatus.Processing)
            throw new InvalidOperationException("Only processing orders can be shipped.");
        Status = OrderStatus.Shipped;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deliver()
    {
        if (Status != OrderStatus.Shipped)
            throw new InvalidOperationException("Only shipped orders can be delivered.");
        Status = OrderStatus.Delivered;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Delivered || Status == OrderStatus.Cancelled)
            throw new InvalidOperationException("Cannot cancel this order.");
        Status = OrderStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    private void RecalculateTotal()
    {
        var total = _items.Sum(i => i.UnitPrice.Amount * i.Quantity);
        TotalAmount = new Money(total);
    }
}
