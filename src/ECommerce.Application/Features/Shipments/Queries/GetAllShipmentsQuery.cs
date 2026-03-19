using AutoMapper;
using ECommerce.Application.Common.Models;
using ECommerce.Domain.Interfaces;
using MediatR;

namespace ECommerce.Application.Features.Shipments.Queries;

public record GetAllShipmentsQuery : IRequest<IReadOnlyList<ShipmentDto>>;

public class GetAllShipmentsQueryHandler : IRequestHandler<GetAllShipmentsQuery, IReadOnlyList<ShipmentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllShipmentsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ShipmentDto>> Handle(GetAllShipmentsQuery request, CancellationToken cancellationToken)
    {
        var shipments = await _unitOfWork.Shipments.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<ShipmentDto>>(shipments);
    }
}

public record GetShipmentByIdQuery(Guid Id) : IRequest<Result<ShipmentDto>>;

public class GetShipmentByIdQueryHandler : IRequestHandler<GetShipmentByIdQuery, Result<ShipmentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetShipmentByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<ShipmentDto>> Handle(GetShipmentByIdQuery request, CancellationToken cancellationToken)
    {
        var shipment = await _unitOfWork.Shipments.GetByIdAsync(request.Id, cancellationToken);
        if (shipment is null) return Result<ShipmentDto>.Failure("Shipment not found.");
        return Result<ShipmentDto>.Success(_mapper.Map<ShipmentDto>(shipment));
    }
}
