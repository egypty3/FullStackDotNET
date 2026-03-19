using ECommerce.Domain.Common;
using ECommerce.Domain.Enums;

namespace ECommerce.Domain.Entities;

public class Coupon : BaseEntity
{
    public string Code { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public DiscountType DiscountType { get; private set; }
    public decimal DiscountValue { get; private set; }
    public decimal? MinOrderAmount { get; private set; }
    public int MaxUses { get; private set; }
    public int CurrentUses { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public bool IsActive { get; private set; } = true;

    public bool IsExpired => DateTime.UtcNow > ExpiresAt;
    public bool IsFullyRedeemed => CurrentUses >= MaxUses;
    public bool IsValid => IsActive && !IsExpired && !IsFullyRedeemed;

    private Coupon() { }

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
            Code = code.ToUpperInvariant(),
            Description = description,
            DiscountType = discountType,
            DiscountValue = discountValue,
            MinOrderAmount = minOrderAmount,
            MaxUses = maxUses,
            CurrentUses = 0,
            ExpiresAt = expiresAt
        };
    }

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

    public void Redeem()
    {
        if (!IsValid)
            throw new InvalidOperationException("Coupon is not valid.");

        CurrentUses++;
        UpdatedAt = DateTime.UtcNow;
    }

    public decimal CalculateDiscount(decimal orderAmount)
    {
        if (!IsValid) return 0;
        if (MinOrderAmount.HasValue && orderAmount < MinOrderAmount.Value) return 0;

        return DiscountType == DiscountType.Percentage
            ? orderAmount * (DiscountValue / 100)
            : Math.Min(DiscountValue, orderAmount);
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }
}
