using ECommerce.Application.Common.Models;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using ECommerce.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace ECommerce.Application.Features.Payments.Commands;

public record CreatePaymentCommand(
    Guid OrderId, decimal Amount, string Method, string? TransactionId
) : IRequest<Result<Guid>>;

public class CreatePaymentCommandValidator : AbstractValidator<CreatePaymentCommand>
{
    public CreatePaymentCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Method).NotEmpty();
    }
}

public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreatePaymentCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Result<Guid>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null) return Result<Guid>.Failure("Order not found.");

        if (!Enum.TryParse<PaymentMethod>(request.Method, true, out var method))
            return Result<Guid>.Failure($"Invalid payment method: {request.Method}");

        var payment = Payment.Create(request.OrderId, request.Amount, method, request.TransactionId);

        await _unitOfWork.Payments.AddAsync(payment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(payment.Id);
    }
}

public record CompletePaymentCommand(Guid Id, string TransactionId) : IRequest<Result<bool>>;

public class CompletePaymentCommandHandler : IRequestHandler<CompletePaymentCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CompletePaymentCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Result<bool>> Handle(CompletePaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await _unitOfWork.Payments.GetByIdAsync(request.Id, cancellationToken);
        if (payment is null) return Result<bool>.Failure("Payment not found.");

        payment.MarkAsCompleted(request.TransactionId);

        await _unitOfWork.Payments.UpdateAsync(payment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
