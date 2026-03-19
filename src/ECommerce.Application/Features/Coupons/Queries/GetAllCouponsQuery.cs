using AutoMapper;
using ECommerce.Application.Common.Models;
using ECommerce.Domain.Interfaces;
using MediatR;

namespace ECommerce.Application.Features.Coupons.Queries;

public record GetAllCouponsQuery : IRequest<IReadOnlyList<CouponDto>>;

public class GetAllCouponsQueryHandler : IRequestHandler<GetAllCouponsQuery, IReadOnlyList<CouponDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllCouponsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<CouponDto>> Handle(GetAllCouponsQuery request, CancellationToken cancellationToken)
    {
        var coupons = await _unitOfWork.Coupons.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<CouponDto>>(coupons);
    }
}

public record GetCouponByIdQuery(Guid Id) : IRequest<Result<CouponDto>>;

public class GetCouponByIdQueryHandler : IRequestHandler<GetCouponByIdQuery, Result<CouponDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCouponByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<CouponDto>> Handle(GetCouponByIdQuery request, CancellationToken cancellationToken)
    {
        var coupon = await _unitOfWork.Coupons.GetByIdAsync(request.Id, cancellationToken);
        if (coupon is null) return Result<CouponDto>.Failure("Coupon not found.");
        return Result<CouponDto>.Success(_mapper.Map<CouponDto>(coupon));
    }
}
