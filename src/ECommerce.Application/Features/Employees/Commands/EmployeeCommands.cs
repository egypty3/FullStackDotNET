using ECommerce.Application.Common.Models;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace ECommerce.Application.Features.Employees.Commands;

public record CreateEmployeeCommand(
    string FirstName, string LastName, string Email, string Phone,
    string Department, string Position, DateTime HireDate, decimal Salary
) : IRequest<Result<Guid>>;

public class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
{
    public CreateEmployeeCommandValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(200);
        RuleFor(x => x.Department).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Position).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Salary).GreaterThanOrEqualTo(0);
    }
}

public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateEmployeeCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Result<Guid>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var existing = await _unitOfWork.Employees.GetByEmailAsync(request.Email, cancellationToken);
        if (existing is not null)
            return Result<Guid>.Failure($"Employee with email '{request.Email}' already exists.");

        var employee = Employee.Create(request.FirstName, request.LastName, request.Email, request.Phone,
            request.Department, request.Position, request.HireDate, request.Salary);

        await _unitOfWork.Employees.AddAsync(employee, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(employee.Id);
    }
}

public record UpdateEmployeeCommand(
    Guid Id, string FirstName, string LastName, string Email, string Phone,
    string Department, string Position, decimal Salary
) : IRequest<Result<bool>>;

public class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
{
    public UpdateEmployeeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(200);
        RuleFor(x => x.Department).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Position).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Salary).GreaterThanOrEqualTo(0);
    }
}

public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateEmployeeCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Result<bool>> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await _unitOfWork.Employees.GetByIdAsync(request.Id, cancellationToken);
        if (employee is null) return Result<bool>.Failure("Employee not found.");

        var duplicate = await _unitOfWork.Employees.GetByEmailAsync(request.Email, cancellationToken);
        if (duplicate is not null && duplicate.Id != request.Id)
            return Result<bool>.Failure($"Email '{request.Email}' is already in use.");

        employee.Update(request.FirstName, request.LastName, request.Email, request.Phone,
            request.Department, request.Position, request.Salary);

        await _unitOfWork.Employees.UpdateAsync(employee, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}

public record DeleteEmployeeCommand(Guid Id) : IRequest<Result<bool>>;

public class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteEmployeeCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Result<bool>> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await _unitOfWork.Employees.GetByIdAsync(request.Id, cancellationToken);
        if (employee is null) return Result<bool>.Failure("Employee not found.");

        employee.Deactivate();
        await _unitOfWork.Employees.UpdateAsync(employee, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
