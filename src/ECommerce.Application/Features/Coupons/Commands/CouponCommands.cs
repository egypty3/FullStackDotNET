using ECommerce.Application.Common.Models;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using ECommerce.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace ECommerce.Application.Features.Coupons.Commands;

public record CreateCouponCommand(
    string Code, string Description, string DiscountType,
    decimal DiscountValue, int MaxUses, DateTime ExpiresAt, decimal? MinOrderAmount
) : IRequest<Result<Guid>>;

public class CreateCouponCommandValidator : AbstractValidator<CreateCouponCommand>
{
    public CreateCouponCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.DiscountType).NotEmpty();
        RuleFor(x => x.DiscountValue).GreaterThan(0);
        RuleFor(x => x.MaxUses).GreaterThan(0);
        RuleFor(x => x.ExpiresAt).GreaterThan(DateTime.UtcNow);
    }
}

public class CreateCouponCommandHandler : IRequestHandler<CreateCouponCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateCouponCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Result<Guid>> Handle(CreateCouponCommand request, CancellationToken cancellationToken)
    {
        var existing = await _unitOfWork.Coupons.GetByCodeAsync(request.Code, cancellationToken);
        if (existing is not null)
            return Result<Guid>.Failure($"Coupon code '{request.Code}' already exists.");

        if (!Enum.TryParse<DiscountType>(request.DiscountType, true, out var discountType))
            return Result<Guid>.Failure($"Invalid discount type: {request.DiscountType}");

        var coupon = Coupon.Create(request.Code, request.Description, discountType,
            request.DiscountValue, request.MaxUses, request.ExpiresAt, request.MinOrderAmount);

        await _unitOfWork.Coupons.AddAsync(coupon, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(coupon.Id);
    }
}

public record UpdateCouponCommand(
    Guid Id, string Description, string DiscountType,
    decimal DiscountValue, int MaxUses, DateTime ExpiresAt, decimal? MinOrderAmount
) : IRequest<Result<bool>>;

public class UpdateCouponCommandValidator : AbstractValidator<UpdateCouponCommand>
{
    public UpdateCouponCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.DiscountValue).GreaterThan(0);
        RuleFor(x => x.MaxUses).GreaterThan(0);
    }
}

public class UpdateCouponCommandHandler : IRequestHandler<UpdateCouponCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCouponCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Result<bool>> Handle(UpdateCouponCommand request, CancellationToken cancellationToken)
    {
        var coupon = await _unitOfWork.Coupons.GetByIdAsync(request.Id, cancellationToken);
        if (coupon is null) return Result<bool>.Failure("Coupon not found.");

        if (!Enum.TryParse<DiscountType>(request.DiscountType, true, out var discountType))
            return Result<bool>.Failure($"Invalid discount type: {request.DiscountType}");

        coupon.Update(request.Description, discountType, request.DiscountValue,
            request.MaxUses, request.ExpiresAt, request.MinOrderAmount);

        await _unitOfWork.Coupons.UpdateAsync(coupon, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}

public record DeleteCouponCommand(Guid Id) : IRequest<Result<bool>>;

public class DeleteCouponCommandHandler : IRequestHandler<DeleteCouponCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCouponCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Result<bool>> Handle(DeleteCouponCommand request, CancellationToken cancellationToken)
    {
        var coupon = await _unitOfWork.Coupons.GetByIdAsync(request.Id, cancellationToken);
        if (coupon is null) return Result<bool>.Failure("Coupon not found.");

        coupon.Deactivate();
        await _unitOfWork.Coupons.UpdateAsync(coupon, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
