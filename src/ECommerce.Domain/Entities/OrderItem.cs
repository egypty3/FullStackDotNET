using ECommerce.Domain.Common;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Domain.Entities;

public class OrderItem : BaseEntity
{
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public Money UnitPrice { get; private set; } = new(0);
    public int Quantity { get; private set; }
    public Money Subtotal => UnitPrice.Multiply(Quantity);

    private OrderItem() { } // EF Core

    public static OrderItem Create(Guid orderId, Guid productId, string productName, Money unitPrice, int quantity)
    {
        return new OrderItem
        {
            OrderId = orderId,
            ProductId = productId,
            ProductName = productName,
            UnitPrice = unitPrice,
            Quantity = quantity
        };
    }

    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new ArgumentException("Quantity must be positive.");
        Quantity = newQuantity;
    }
}
