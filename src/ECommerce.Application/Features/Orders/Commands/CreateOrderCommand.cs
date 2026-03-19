using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using ECommerce.Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace ECommerce.Application.Features.Orders.Commands;

public record CreateOrderCommand(
    List<OrderItemRequest> Items,
    string Street,
    string City,
    string State,
    string ZipCode,
    string Country
) : IRequest<Result<Guid>>;

public record OrderItemRequest(Guid ProductId, int Quantity);

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.Items).NotEmpty().WithMessage("Order must contain at least one item.");
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(x => x.ProductId).NotEmpty();
            item.RuleFor(x => x.Quantity).GreaterThan(0);
        });
        RuleFor(x => x.Street).NotEmpty();
        RuleFor(x => x.City).NotEmpty();
        RuleFor(x => x.Country).NotEmpty();
    }
}

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public CreateOrderCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<Guid>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var address = new Address(request.Street, request.City, request.State, request.ZipCode, request.Country);
        var order = Order.Create(_currentUser.UserId!, _currentUser.Email!, address);

        foreach (var item in request.Items)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId, cancellationToken);
            if (product is null)
                return Result<Guid>.Failure($"Product {item.ProductId} not found.");
            if (!product.IsActive)
                return Result<Guid>.Failure($"Product '{product.Name}' is no longer available.");
            if (product.StockQuantity < item.Quantity)
                return Result<Guid>.Failure($"Insufficient stock for '{product.Name}'. Available: {product.StockQuantity}");

            order.AddItem(product, item.Quantity);
            product.ReduceStock(item.Quantity);
        }

        order.Confirm();

        await _unitOfWork.Orders.AddAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(order.Id);
    }
}
