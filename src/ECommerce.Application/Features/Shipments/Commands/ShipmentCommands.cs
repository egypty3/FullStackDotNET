using ECommerce.Application.Common.Models;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace ECommerce.Application.Features.Shipments.Commands;

public record CreateShipmentCommand(
    Guid OrderId, Guid ShipperId, string TrackingNumber, decimal ShippingCost,
    DateTime? EstimatedDeliveryDate, string? Notes
) : IRequest<Result<Guid>>;

public class CreateShipmentCommandValidator : AbstractValidator<CreateShipmentCommand>
{
    public CreateShipmentCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.ShipperId).NotEmpty();
        RuleFor(x => x.TrackingNumber).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ShippingCost).GreaterThanOrEqualTo(0);
    }
}

public class CreateShipmentCommandHandler : IRequestHandler<CreateShipmentCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateShipmentCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Result<Guid>> Handle(CreateShipmentCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null) return Result<Guid>.Failure("Order not found.");

        var shipper = await _unitOfWork.Shippers.GetByIdAsync(request.ShipperId, cancellationToken);
        if (shipper is null) return Result<Guid>.Failure("Shipper not found.");

        var shipment = Shipment.Create(request.OrderId, request.ShipperId, request.TrackingNumber,
            request.ShippingCost, request.EstimatedDeliveryDate, request.Notes);

        await _unitOfWork.Shipments.AddAsync(shipment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(shipment.Id);
    }
}

public record UpdateShipmentStatusCommand(Guid Id, string Status) : IRequest<Result<bool>>;

public class UpdateShipmentStatusCommandHandler : IRequestHandler<UpdateShipmentStatusCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateShipmentStatusCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Result<bool>> Handle(UpdateShipmentStatusCommand request, CancellationToken cancellationToken)
    {
        var shipment = await _unitOfWork.Shipments.GetByIdAsync(request.Id, cancellationToken);
        if (shipment is null) return Result<bool>.Failure("Shipment not found.");

        switch (request.Status.ToLowerInvariant())
        {
            case "shipped": shipment.MarkAsShipped(); break;
            case "intransit": shipment.MarkInTransit(); break;
            case "outfordelivery": shipment.MarkOutForDelivery(); break;
            case "delivered": shipment.MarkAsDelivered(); break;
            case "returned": shipment.MarkAsReturned(); break;
            default: return Result<bool>.Failure($"Invalid status: {request.Status}");
        }

        await _unitOfWork.Shipments.UpdateAsync(shipment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
