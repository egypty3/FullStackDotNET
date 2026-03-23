namespace ECommerce.Domain.Enums;

/// <summary>
/// Represents the lifecycle stages of an order in the ECommerce system.
/// Orders follow a state machine pattern: Pending -> Confirmed -> Processing -> Shipped -> Delivered.
/// Orders can also be Cancelled (from most states) or Refunded (after payment).
/// The integer values (0-6) are used for database storage and serialization.
/// </summary>
public enum OrderStatus
{
    /// <summary>Order has been created but not yet confirmed by the system or customer.</summary>
    Pending = 0,
    /// <summary>Order has been confirmed and is awaiting payment processing.</summary>
    Confirmed = 1,
    /// <summary>Payment received; order is being prepared for shipment.</summary>
    Processing = 2,
    /// <summary>Order has been handed off to a shipping carrier.</summary>
    Shipped = 3,
    /// <summary>Order has been successfully delivered to the customer.</summary>
    Delivered = 4,
    /// <summary>Order was cancelled before delivery (by customer or system).</summary>
    Cancelled = 5,
    /// <summary>Payment was refunded to the customer after cancellation or return.</summary>
    Refunded = 6
}

/// <summary>
/// Represents the status of a payment transaction.
/// Tracks the payment lifecycle from initiation through completion or failure.
/// </summary>
public enum PaymentStatus
{
    /// <summary>Payment has been initiated but not yet processed by the payment gateway.</summary>
    Pending = 0,
    /// <summary>Payment was successfully processed and funds have been captured.</summary>
    Completed = 1,
    /// <summary>Payment processing failed (e.g., insufficient funds, declined card).</summary>
    Failed = 2,
    /// <summary>A previously completed payment has been refunded to the customer.</summary>
    Refunded = 3
}

/// <summary>
/// Represents predefined product categories for organizing the product catalog.
/// Used for filtering, searching, and displaying products in category-based navigation.
/// </summary>
public enum ProductCategory
{
    /// <summary>Electronic devices and accessories (phones, laptops, etc.).</summary>
    Electronics = 0,
    /// <summary>Apparel, shoes, and fashion accessories.</summary>
    Clothing = 1,
    /// <summary>Physical and digital books, magazines, and publications.</summary>
    Books = 2,
    /// <summary>Home improvement, furniture, gardening tools, and decor.</summary>
    HomeAndGarden = 3,
    /// <summary>Sporting goods, fitness equipment, and outdoor gear.</summary>
    Sports = 4,
    /// <summary>Food items, beverages, and grocery products.</summary>
    Food = 5,
    /// <summary>Products that don't fit into any other predefined category.</summary>
    Other = 6
}

/// <summary>
/// Represents the tracking stages of a shipment from dispatch to delivery.
/// Follows the typical logistics lifecycle of a package being shipped to a customer.
/// </summary>
public enum ShipmentStatus
{
    /// <summary>Shipment record created but package has not yet been dispatched.</summary>
    Pending = 0,
    /// <summary>Package has been dispatched from the warehouse/fulfillment center.</summary>
    Shipped = 1,
    /// <summary>Package is in transit between distribution centers.</summary>
    InTransit = 2,
    /// <summary>Package is on the final delivery vehicle heading to the customer.</summary>
    OutForDelivery = 3,
    /// <summary>Package has been successfully delivered to the customer.</summary>
    Delivered = 4,
    /// <summary>Package was returned (delivery failed, customer refused, or return initiated).</summary>
    Returned = 5
}

/// <summary>
/// Represents the available payment methods that customers can use to pay for orders.
/// Each method may have different processing flows and fees in the payment gateway integration.
/// </summary>
public enum PaymentMethod
{
    /// <summary>Payment via credit card (Visa, MasterCard, Amex, etc.).</summary>
    CreditCard = 0,
    /// <summary>Payment via debit card linked directly to a bank account.</summary>
    DebitCard = 1,
    /// <summary>Payment via PayPal digital wallet.</summary>
    PayPal = 2,
    /// <summary>Direct bank-to-bank electronic transfer.</summary>
    BankTransfer = 3,
    /// <summary>Payment collected in cash when the order is delivered (COD).</summary>
    CashOnDelivery = 4
}

/// <summary>
/// Defines how a coupon discount is calculated and applied to an order.
/// Used by the Coupon entity's CalculateDiscount method to determine the final discount amount.
/// </summary>
public enum DiscountType
{
    /// <summary>Discount is a percentage of the order total (e.g., 10% off). The DiscountValue represents the percentage.</summary>
    Percentage = 0,
    /// <summary>Discount is a fixed currency amount subtracted from the order total (e.g., $5 off). The DiscountValue represents the fixed amount.</summary>
    FixedAmount = 1
}
