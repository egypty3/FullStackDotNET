using ECommerce.Application.Common.Models;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using ECommerce.Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace ECommerce.Application.Features.Customers.Commands;

public record CreateCustomerCommand(
    string FirstName, string LastName, string Email, string Phone,
    string? Street, string? City, string? State, string? ZipCode, string? Country,
    DateTime? DateOfBirth
) : IRequest<Result<Guid>>;

public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(200);
        RuleFor(x => x.Phone).MaximumLength(20);
    }
}

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateCustomerCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Result<Guid>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var existing = await _unitOfWork.Customers.GetByEmailAsync(request.Email, cancellationToken);
        if (existing is not null)
            return Result<Guid>.Failure($"Customer with email '{request.Email}' already exists.");

        Address? address = null;
        if (!string.IsNullOrWhiteSpace(request.Street) && !string.IsNullOrWhiteSpace(request.City) && !string.IsNullOrWhiteSpace(request.Country))
            address = new Address(request.Street, request.City, request.State ?? "", request.ZipCode ?? "", request.Country);

        var customer = Customer.Create(request.FirstName, request.LastName, request.Email, request.Phone, address, request.DateOfBirth);

        await _unitOfWork.Customers.AddAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(customer.Id);
    }
}

public record UpdateCustomerCommand(
    Guid Id, string FirstName, string LastName, string Email, string Phone,
    string? Street, string? City, string? State, string? ZipCode, string? Country,
    DateTime? DateOfBirth
) : IRequest<Result<bool>>;

public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
{
    public UpdateCustomerCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(200);
    }
}

public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCustomerCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Result<bool>> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.Customers.GetByIdAsync(request.Id, cancellationToken);
        if (customer is null) return Result<bool>.Failure("Customer not found.");

        var duplicate = await _unitOfWork.Customers.GetByEmailAsync(request.Email, cancellationToken);
        if (duplicate is not null && duplicate.Id != request.Id)
            return Result<bool>.Failure($"Email '{request.Email}' is already in use.");

        Address? address = null;
        if (!string.IsNullOrWhiteSpace(request.Street) && !string.IsNullOrWhiteSpace(request.City) && !string.IsNullOrWhiteSpace(request.Country))
            address = new Address(request.Street, request.City, request.State ?? "", request.ZipCode ?? "", request.Country);

        customer.Update(request.FirstName, request.LastName, request.Email, request.Phone, address, request.DateOfBirth);

        await _unitOfWork.Customers.UpdateAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}

public record DeleteCustomerCommand(Guid Id) : IRequest<Result<bool>>;

public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCustomerCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Result<bool>> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.Customers.GetByIdAsync(request.Id, cancellationToken);
        if (customer is null) return Result<bool>.Failure("Customer not found.");

        customer.Deactivate();
        await _unitOfWork.Customers.UpdateAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
