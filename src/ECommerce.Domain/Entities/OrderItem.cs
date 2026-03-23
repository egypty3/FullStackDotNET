using ECommerce.Domain.Common;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Domain.Entities;

/// <summary>
/// Represents a single line item within an Order — linking a product to a quantity and price.
/// 
/// OrderItem is a child entity of the Order aggregate. It is never created or managed independently —
/// items are always added/removed through the parent Order entity's AddItem()/RemoveItem() methods,
/// which enforce business rules (e.g., orders can only be modified while in Pending status).
/// 
/// Uses the Snapshot Pattern: The ProductName and UnitPrice are captured at the time the item is added
/// to the order. This means the order preserves the product's name and price as they were at purchase time,
/// even if the product is later renamed or repriced. This is essential for accurate order history.
/// 
/// Inherits Id, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy from BaseEntity.
/// </summary>
public class OrderItem : BaseEntity
{
    /// <summary>Foreign key to the parent Order. Links this item to its containing order.</summary>
    public Guid OrderId { get; private set; }

    /// <summary>
    /// Foreign key to the Product. Used for inventory/reporting queries.
    /// Note: The actual product details (name, price) are snapshotted below, not fetched via this FK.
    /// </summary>
    public Guid ProductId { get; private set; }

    /// <summary>
    /// Snapshot of the product name at the time the item was added to the order.
    /// Preserved even if the product is later renamed, ensuring order history accuracy.
    /// </summary>
    public string ProductName { get; private set; } = string.Empty;

    /// <summary>
    /// Snapshot of the product's unit price at the time the item was added.
    /// Stored as a Money value object to include currency information.
    /// Preserved even if the product's price changes later.
    /// </summary>
    public Money UnitPrice { get; private set; } = new(0);

    /// <summary>The number of units of this product in the order. Must be positive (enforced by UpdateQuantity).</summary>
    public int Quantity { get; private set; }

    /// <summary>
    /// Computed property: calculates the line item total (UnitPrice * Quantity).
    /// Uses the Money.Multiply() method to maintain currency consistency.
    /// Not stored in the database — computed on-the-fly whenever accessed.
    /// </summary>
    public Money Subtotal => UnitPrice.Multiply(Quantity);

    /// <summary>Private parameterless constructor required by EF Core for database materialization.</summary>
    private OrderItem() { } // EF Core

    /// <summary>
    /// Factory method to create a new OrderItem.
    /// Typically called internally by Order.AddItem() — not directly by application code.
    /// Captures a snapshot of the product's current name and price.
    /// </summary>
    /// <param name="orderId">The ID of the parent order.</param>
    /// <param name="productId">The ID of the product being ordered.</param>
    /// <param name="productName">The product's current name (snapshotted for order history).</param>
    /// <param name="unitPrice">The product's current price (snapshotted for order history).</param>
    /// <param name="quantity">The number of units to order.</param>
    /// <returns>A new OrderItem instance with the captured product details.</returns>
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

    /// <summary>
    /// Updates the quantity of this order item.
    /// Called by Order.AddItem() when the same product is added again (quantities are combined).
    /// Validates that the new quantity is positive.
    /// </summary>
    /// <param name="newQuantity">The new quantity (must be > 0).</param>
    /// <exception cref="ArgumentException">Thrown if newQuantity is <= 0.</exception>
    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new ArgumentException("Quantity must be positive.");
        Quantity = newQuantity;
    }
}
