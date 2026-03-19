namespace ECommerce.Application.Features.Coupons.Queries;

public record CouponDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string DiscountType { get; init; } = string.Empty;
    public decimal DiscountValue { get; init; }
    public decimal? MinOrderAmount { get; init; }
    public int MaxUses { get; init; }
    public int CurrentUses { get; init; }
    public DateTime ExpiresAt { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
}
