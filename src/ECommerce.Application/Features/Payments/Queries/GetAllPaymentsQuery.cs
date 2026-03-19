using AutoMapper;
using ECommerce.Application.Common.Models;
using ECommerce.Domain.Interfaces;
using MediatR;

namespace ECommerce.Application.Features.Payments.Queries;

public record GetAllPaymentsQuery : IRequest<IReadOnlyList<PaymentDto>>;

public class GetAllPaymentsQueryHandler : IRequestHandler<GetAllPaymentsQuery, IReadOnlyList<PaymentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllPaymentsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<PaymentDto>> Handle(GetAllPaymentsQuery request, CancellationToken cancellationToken)
    {
        var payments = await _unitOfWork.Payments.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<PaymentDto>>(payments);
    }
}

public record GetPaymentByIdQuery(Guid Id) : IRequest<Result<PaymentDto>>;

public class GetPaymentByIdQueryHandler : IRequestHandler<GetPaymentByIdQuery, Result<PaymentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetPaymentByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<PaymentDto>> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
    {
        var payment = await _unitOfWork.Payments.GetByIdAsync(request.Id, cancellationToken);
        if (payment is null) return Result<PaymentDto>.Failure("Payment not found.");
        return Result<PaymentDto>.Success(_mapper.Map<PaymentDto>(payment));
    }
}
