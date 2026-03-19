using AutoMapper;
using ECommerce.Application.Common.Models;
using ECommerce.Domain.Interfaces;
using MediatR;

namespace ECommerce.Application.Features.Shippers.Queries;

public record GetAllShippersQuery : IRequest<IReadOnlyList<ShipperDto>>;

public class GetAllShippersQueryHandler : IRequestHandler<GetAllShippersQuery, IReadOnlyList<ShipperDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllShippersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ShipperDto>> Handle(GetAllShippersQuery request, CancellationToken cancellationToken)
    {
        var shippers = await _unitOfWork.Shippers.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<ShipperDto>>(shippers);
    }
}

public record GetShipperByIdQuery(Guid Id) : IRequest<Result<ShipperDto>>;

public class GetShipperByIdQueryHandler : IRequestHandler<GetShipperByIdQuery, Result<ShipperDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetShipperByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<ShipperDto>> Handle(GetShipperByIdQuery request, CancellationToken cancellationToken)
    {
        var shipper = await _unitOfWork.Shippers.GetByIdAsync(request.Id, cancellationToken);
        if (shipper is null) return Result<ShipperDto>.Failure("Shipper not found.");
        return Result<ShipperDto>.Success(_mapper.Map<ShipperDto>(shipper));
    }
}
