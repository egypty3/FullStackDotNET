using ECommerce.Application.Common.Models;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace ECommerce.Application.Features.Shippers.Commands;

public record CreateShipperCommand(
    string CompanyName, string Phone, string Email, string ContactPerson, string? Website
) : IRequest<Result<Guid>>;

public class CreateShipperCommandValidator : AbstractValidator<CreateShipperCommand>
{
    public CreateShipperCommandValidator()
    {
        RuleFor(x => x.CompanyName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Email).MaximumLength(200);
        RuleFor(x => x.ContactPerson).MaximumLength(200);
    }
}

public class CreateShipperCommandHandler : IRequestHandler<CreateShipperCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateShipperCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Result<Guid>> Handle(CreateShipperCommand request, CancellationToken cancellationToken)
    {
        var existing = await _unitOfWork.Shippers.GetByNameAsync(request.CompanyName, cancellationToken);
        if (existing is not null)
            return Result<Guid>.Failure($"Shipper '{request.CompanyName}' already exists.");

        var shipper = Shipper.Create(request.CompanyName, request.Phone, request.Email, request.ContactPerson, request.Website);

        await _unitOfWork.Shippers.AddAsync(shipper, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(shipper.Id);
    }
}

public record UpdateShipperCommand(
    Guid Id, string CompanyName, string Phone, string Email, string ContactPerson, string? Website
) : IRequest<Result<bool>>;

public class UpdateShipperCommandValidator : AbstractValidator<UpdateShipperCommand>
{
    public UpdateShipperCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.CompanyName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(20);
    }
}

public class UpdateShipperCommandHandler : IRequestHandler<UpdateShipperCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateShipperCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Result<bool>> Handle(UpdateShipperCommand request, CancellationToken cancellationToken)
    {
        var shipper = await _unitOfWork.Shippers.GetByIdAsync(request.Id, cancellationToken);
        if (shipper is null) return Result<bool>.Failure("Shipper not found.");

        var duplicate = await _unitOfWork.Shippers.GetByNameAsync(request.CompanyName, cancellationToken);
        if (duplicate is not null && duplicate.Id != request.Id)
            return Result<bool>.Failure($"Shipper '{request.CompanyName}' already exists.");

        shipper.Update(request.CompanyName, request.Phone, request.Email, request.ContactPerson, request.Website);

        await _unitOfWork.Shippers.UpdateAsync(shipper, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}

public record DeleteShipperCommand(Guid Id) : IRequest<Result<bool>>;

public class DeleteShipperCommandHandler : IRequestHandler<DeleteShipperCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteShipperCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Result<bool>> Handle(DeleteShipperCommand request, CancellationToken cancellationToken)
    {
        var shipper = await _unitOfWork.Shippers.GetByIdAsync(request.Id, cancellationToken);
        if (shipper is null) return Result<bool>.Failure("Shipper not found.");

        shipper.Deactivate();
        await _unitOfWork.Shippers.UpdateAsync(shipper, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
