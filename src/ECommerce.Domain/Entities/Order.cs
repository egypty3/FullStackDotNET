using ECommerce.Domain.Common;
using ECommerce.Domain.Enums;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Domain.Entities;

/// <summary>
/// Represents a customer's order — the central aggregate root of the ECommerce domain.
/// 
/// An Order acts as an Aggregate Root in DDD terms: it controls access to its child OrderItem entities
/// and enforces all business invariants for the order lifecycle.
/// 
/// Order Lifecycle (State Machine):
///   Pending -> Confirmed -> Processing (after payment) -> Shipped -> Delivered
///              |                                          |
///              +-------> Cancelled <----------------------+
/// 
/// Key business rules enforced:
/// - Items can only be added/removed while the order is in Pending status.
/// - An order cannot be confirmed if it has no items.
/// - The total amount is automatically recalculated whenever items are added or removed.
/// - Payment status and order status are tracked independently but linked (payment triggers Processing).
/// - State transitions are guarded by the current status (e.g., only Processing orders can be shipped).
/// 
/// The _items field is a private list exposed as IReadOnlyCollection to prevent external code
/// from bypassing business rules by directly modifying the collection.
/// 
/// Inherits Id, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy from BaseEntity.
/// </summary>
public class Order : BaseEntity
{
    /// <summary>Identifier of the customer who placed this order. Stored as string to accommodate different ID formats.</summary>
    public string CustomerId { get; private set; } = string.Empty;

    /// <summary>Customer's email address at the time of order creation. Stored on the order to preserve it even if the customer updates their email later.</summary>
    public string CustomerEmail { get; private set; } = string.Empty;

    /// <summary>
    /// Current status of the order in its lifecycle.
    /// Defaults to Pending when a new order is created.
    /// Transitions are managed by domain methods (Confirm, MarkAsPaid, Ship, Deliver, Cancel).
    /// </summary>
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;

    /// <summary>
    /// The delivery address for this order, stored as an Address value object.
    /// Captured at order creation time so it's preserved even if the customer changes their address later.
    /// The null-forgiving operator (null!) is used because EF Core will always populate this from the database.
    /// </summary>
    public Address ShippingAddress { get; private set; } = null!;

    /// <summary>
    /// The total monetary value of all items in the order, stored as a Money value object.
    /// Automatically recalculated by RecalculateTotal() whenever items are added or removed.
    /// </summary>
    public Money TotalAmount { get; private set; } = new(0);

    /// <summary>Tracks whether payment has been received for this order. Independent of OrderStatus.</summary>
    public PaymentStatus PaymentStatus { get; private set; } = PaymentStatus.Pending;

    /// <summary>The external transaction ID from the payment gateway (e.g., Stripe charge ID). Set when payment completes.</summary>
    public string? PaymentTransactionId { get; private set; }

    /// <summary>Timestamp of when payment was successfully received. Null until payment is completed.</summary>
    public DateTime? PaidAt { get; private set; }

    /// <summary>
    /// Private backing field for order items. Using a private list ensures that items can only be
    /// added/removed through the Order's domain methods (AddItem, RemoveItem), which enforce business rules.
    /// </summary>
    private readonly List<OrderItem> _items = new();

    /// <summary>
    /// Public read-only view of the order's line items.
    /// AsReadOnly() returns a wrapper that prevents external code from casting back to List and modifying it.
    /// This enforces the Aggregate Root pattern — all modifications go through the Order entity.
    /// </summary>
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    /// <summary>Private parameterless constructor required by EF Core for database materialization.</summary>
    private Order() { } // EF Core

    /// <summary>
    /// Factory method to create a new Order in Pending status.
    /// The order starts empty (no items) — items are added via AddItem() after creation.
    /// </summary>
    /// <param name="customerId">The ID of the customer placing the order (required).</param>
    /// <param name="customerEmail">The customer's email for order notifications (required).</param>
    /// <param name="shippingAddress">The delivery address for this order.</param>
    /// <returns>A new Order in Pending status with zero items.</returns>
    /// <exception cref="ArgumentException">Thrown if customerId or customerEmail is empty.</exception>
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

    /// <summary>
    /// Adds a product to the order or increases the quantity if the product is already in the order.
    /// Can only be called while the order is in Pending status (before confirmation).
    /// Automatically recalculates the order total after adding the item.
    /// 
    /// If the same product already exists in the order, the quantities are combined (idempotent addition).
    /// </summary>
    /// <param name="product">The product entity to add (used to capture Id, Name, and Price).</param>
    /// <param name="quantity">The number of units to add (must be positive).</param>
    /// <exception cref="InvalidOperationException">Thrown if the order is not in Pending status.</exception>
    /// <exception cref="ArgumentException">Thrown if quantity is <= 0.</exception>
    public void AddItem(Product product, int quantity)
    {
        // Guard: Only pending orders can be modified
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Cannot modify a confirmed order.");
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive.");

        // Check if this product already exists in the order
        var existingItem = _items.FirstOrDefault(i => i.ProductId == product.Id);
        if (existingItem != null)
        {
            // Product already in order — increase the quantity instead of adding a duplicate line item
            existingItem.UpdateQuantity(existingItem.Quantity + quantity);
        }
        else
        {
            // New product — create a new OrderItem capturing current product details (snapshot pattern)
            _items.Add(OrderItem.Create(Id, product.Id, product.Name, product.Price, quantity));
        }

        // Recalculate the order total to reflect the added/updated item
        RecalculateTotal();
    }

    /// <summary>
    /// Removes a product from the order by its product ID.
    /// Can only be called while the order is in Pending status.
    /// Automatically recalculates the order total after removal.
    /// </summary>
    /// <param name="productId">The ID of the product to remove from the order.</param>
    /// <exception cref="InvalidOperationException">Thrown if the order is not Pending or if the item is not found.</exception>
    public void RemoveItem(Guid productId)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Cannot modify a confirmed order.");

        // Find the item or throw if it doesn't exist
        var item = _items.FirstOrDefault(i => i.ProductId == productId)
            ?? throw new InvalidOperationException("Item not found in order.");

        _items.Remove(item);
        RecalculateTotal();
    }

    /// <summary>
    /// Transitions the order from Pending to Confirmed status.
    /// Business rules:
    /// - Only Pending orders can be confirmed (prevents double-confirmation).
    /// - Orders must contain at least one item (cannot confirm an empty order).
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if order is not Pending or has no items.</exception>
    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Only pending orders can be confirmed.");
        if (!_items.Any())
            throw new InvalidOperationException("Cannot confirm an empty order.");

        Status = OrderStatus.Confirmed;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Records a successful payment and transitions the order to Processing status.
    /// This links the external payment gateway transaction to the order.
    /// Called by the application layer after receiving payment confirmation from the payment service.
    /// </summary>
    /// <param name="transactionId">The external payment transaction ID from the payment gateway.</param>
    public void MarkAsPaid(string transactionId)
    {
        PaymentStatus = PaymentStatus.Completed;
        PaymentTransactionId = transactionId;
        PaidAt = DateTime.UtcNow;
        // Payment triggers the transition from Confirmed to Processing
        Status = OrderStatus.Processing;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Transitions the order from Processing to Shipped status.
    /// Indicates the order has been handed off to a shipping carrier.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the order is not in Processing status.</exception>
    public void Ship()
    {
        if (Status != OrderStatus.Processing)
            throw new InvalidOperationException("Only processing orders can be shipped.");
        Status = OrderStatus.Shipped;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Transitions the order from Shipped to Delivered status.
    /// Indicates the customer has received the order.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the order is not in Shipped status.</exception>
    public void Deliver()
    {
        if (Status != OrderStatus.Shipped)
            throw new InvalidOperationException("Only shipped orders can be delivered.");
        Status = OrderStatus.Delivered;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Cancels the order. Can be called from most states except Delivered and already Cancelled.
    /// This prevents cancelling orders that have already been completed or are already cancelled.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the order is Delivered or already Cancelled.</exception>
    public void Cancel()
    {
        if (Status == OrderStatus.Delivered || Status == OrderStatus.Cancelled)
            throw new InvalidOperationException("Cannot cancel this order.");
        Status = OrderStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Private helper that recalculates the TotalAmount by summing all order items (unit price * quantity).
    /// Called internally whenever items are added or removed to keep the total in sync.
    /// Creates a new Money value object (immutable) with the computed total.
    /// </summary>
    private void RecalculateTotal()
    {
        var total = _items.Sum(i => i.UnitPrice.Amount * i.Quantity);
        TotalAmount = new Money(total);
    }
}
