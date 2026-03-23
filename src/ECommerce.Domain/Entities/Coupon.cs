using ECommerce.Domain.Common;
using ECommerce.Domain.Enums;

namespace ECommerce.Domain.Entities;

/// <summary>
/// Represents a discount coupon that customers can apply to orders for price reductions.
/// 
/// Coupons support two discount types (via DiscountType enum):
/// - Percentage: Reduces the order total by a percentage (e.g., 10% off)
/// - FixedAmount: Reduces the order total by a fixed dollar amount (e.g., $5 off)
/// 
/// Business rules enforced by this entity:
/// - Coupon codes are stored in uppercase for case-insensitive matching.
/// - Coupons have a maximum number of uses (MaxUses) and track current redemptions (CurrentUses).
/// - Coupons have an expiration date and cannot be used after that date.
/// - Coupons can optionally require a minimum order amount before the discount applies.
/// - The IsValid computed property combines all validity checks (active, not expired, not fully redeemed).
/// - Supports soft-delete via IsActive flag.
/// 
/// Inherits Id, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy from BaseEntity.
/// </summary>
public class Coupon : BaseEntity
{
    /// <summary>The unique coupon code that customers enter at checkout (stored in uppercase, e.g., "SAVE10").</summary>
    public string Code { get; private set; } = string.Empty;

    /// <summary>Human-readable description of the coupon offer (e.g., "10% off your first order").</summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>Determines how the discount is calculated — either as a Percentage or FixedAmount.</summary>
    public DiscountType DiscountType { get; private set; }

    /// <summary>
    /// The numeric discount value. Interpretation depends on DiscountType:
    /// - If Percentage: this is the percentage (e.g., 10 means 10% off)
    /// - If FixedAmount: this is the dollar amount (e.g., 5.00 means $5 off)
    /// </summary>
    public decimal DiscountValue { get; private set; }

    /// <summary>
    /// Optional minimum order amount required for the coupon to be applicable.
    /// If null, the coupon can be applied to orders of any amount.
    /// If set (e.g., $50), orders below this threshold won't receive the discount.
    /// </summary>
    public decimal? MinOrderAmount { get; private set; }

    /// <summary>The maximum number of times this coupon can be redeemed across all customers.</summary>
    public int MaxUses { get; private set; }

    /// <summary>The number of times this coupon has been redeemed so far. Incremented by Redeem().</summary>
    public int CurrentUses { get; private set; }

    /// <summary>The date and time (UTC) after which the coupon can no longer be used.</summary>
    public DateTime ExpiresAt { get; private set; }

    /// <summary>Soft-delete flag. When false, the coupon is disabled regardless of expiry or usage.</summary>
    public bool IsActive { get; private set; } = true;

    /// <summary>Computed property: returns true if the current UTC time is past the expiration date.</summary>
    public bool IsExpired => DateTime.UtcNow > ExpiresAt;

    /// <summary>Computed property: returns true if the coupon has been used the maximum number of times.</summary>
    public bool IsFullyRedeemed => CurrentUses >= MaxUses;

    /// <summary>
    /// Computed property that combines all validity checks.
    /// A coupon is valid only if it is active AND not expired AND not fully redeemed.
    /// Used by Redeem() and CalculateDiscount() to guard against invalid usage.
    /// </summary>
    public bool IsValid => IsActive && !IsExpired && !IsFullyRedeemed;

    /// <summary>Private parameterless constructor required by EF Core for database materialization.</summary>
    private Coupon() { }

    /// <summary>
    /// Factory method to create a new Coupon with validation.
    /// Validates that the code is not empty, discount value is positive, and max uses is positive.
    /// The coupon code is automatically converted to uppercase for consistent matching.
    /// </summary>
    /// <param name="code">The unique coupon code (required, will be uppercased).</param>
    /// <param name="description">A description of the coupon offer.</param>
    /// <param name="discountType">Whether the discount is a percentage or fixed amount.</param>
    /// <param name="discountValue">The discount value (must be positive).</param>
    /// <param name="maxUses">Maximum number of redemptions allowed (must be positive).</param>
    /// <param name="expiresAt">The expiration date/time for the coupon.</param>
    /// <param name="minOrderAmount">Optional minimum order amount required to use the coupon.</param>
    /// <returns>A new Coupon instance.</returns>
    /// <exception cref="ArgumentException">Thrown if code is empty, discountValue <= 0, or maxUses <= 0.</exception>
    public static Coupon Create(string code, string description, DiscountType discountType,
        decimal discountValue, int maxUses, DateTime expiresAt, decimal? minOrderAmount = null)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Coupon code is required.");
        if (discountValue <= 0)
            throw new ArgumentException("Discount value must be positive.");
        if (maxUses <= 0)
            throw new ArgumentException("Max uses must be positive.");

        return new Coupon
        {
            Code = code.ToUpperInvariant(), // Normalize to uppercase for case-insensitive lookups
            Description = description,
            DiscountType = discountType,
            DiscountValue = discountValue,
            MinOrderAmount = minOrderAmount,
            MaxUses = maxUses,
            CurrentUses = 0, // New coupons start with zero redemptions
            ExpiresAt = expiresAt
        };
    }

    /// <summary>
    /// Updates the coupon's configuration (description, discount rules, limits, and expiration).
    /// Note: The coupon Code cannot be changed after creation to maintain referential integrity.
    /// </summary>
    /// <param name="description">Updated description.</param>
    /// <param name="discountType">Updated discount type.</param>
    /// <param name="discountValue">Updated discount value.</param>
    /// <param name="maxUses">Updated maximum redemptions.</param>
    /// <param name="expiresAt">Updated expiration date.</param>
    /// <param name="minOrderAmount">Updated minimum order amount (null to remove the requirement).</param>
    public void Update(string description, DiscountType discountType, decimal discountValue,
        int maxUses, DateTime expiresAt, decimal? minOrderAmount)
    {
        Description = description;
        DiscountType = discountType;
        DiscountValue = discountValue;
        MaxUses = maxUses;
        ExpiresAt = expiresAt;
        MinOrderAmount = minOrderAmount;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Records a coupon redemption by incrementing the CurrentUses counter.
    /// Can only be called when the coupon IsValid (active, not expired, not fully redeemed).
    /// This is typically called during checkout after the discount has been applied to an order.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the coupon is not in a valid state for redemption.</exception>
    public void Redeem()
    {
        if (!IsValid)
            throw new InvalidOperationException("Coupon is not valid.");

        CurrentUses++;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Calculates the discount amount for a given order total.
    /// Returns 0 if the coupon is invalid or if the order doesn't meet the minimum amount requirement.
    /// 
    /// For Percentage discounts: returns orderAmount * (DiscountValue / 100)
    ///   e.g., $100 order with 10% discount = $10 discount
    /// 
    /// For FixedAmount discounts: returns the lesser of DiscountValue and orderAmount
    ///   (ensures the discount never exceeds the order total, preventing negative totals)
    ///   e.g., $5 discount on $3 order = $3 discount (capped at order amount)
    /// </summary>
    /// <param name="orderAmount">The total order amount before discount.</param>
    /// <returns>The calculated discount amount, or 0 if the coupon cannot be applied.</returns>
    public decimal CalculateDiscount(decimal orderAmount)
    {
        // Return zero if coupon is invalid (expired, fully redeemed, or deactivated)
        if (!IsValid) return 0;

        // Return zero if the order doesn't meet the minimum amount threshold
        if (MinOrderAmount.HasValue && orderAmount < MinOrderAmount.Value) return 0;

        return DiscountType == DiscountType.Percentage
            ? orderAmount * (DiscountValue / 100)                // Percentage: calculate % of order
            : Math.Min(DiscountValue, orderAmount);              // Fixed: cap at order amount to prevent negative totals
    }

    /// <summary>
    /// Disables the coupon by setting IsActive to false.
    /// A deactivated coupon will fail the IsValid check and cannot be redeemed or applied.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Re-enables a previously deactivated coupon by setting IsActive back to true.
    /// Note: The coupon may still be invalid if it has expired or reached max uses.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }
}
