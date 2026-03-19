using AutoMapper;
using ECommerce.Application.Common.Models;
using ECommerce.Domain.Interfaces;
using MediatR;

namespace ECommerce.Application.Features.Employees.Queries;

public record GetAllEmployeesQuery : IRequest<IReadOnlyList<EmployeeDto>>;

public class GetAllEmployeesQueryHandler : IRequestHandler<GetAllEmployeesQuery, IReadOnlyList<EmployeeDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllEmployeesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<EmployeeDto>> Handle(GetAllEmployeesQuery request, CancellationToken cancellationToken)
    {
        var employees = await _unitOfWork.Employees.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<EmployeeDto>>(employees);
    }
}

public record GetEmployeeByIdQuery(Guid Id) : IRequest<Result<EmployeeDto>>;

public class GetEmployeeByIdQueryHandler : IRequestHandler<GetEmployeeByIdQuery, Result<EmployeeDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetEmployeeByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<EmployeeDto>> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {
        var employee = await _unitOfWork.Employees.GetByIdAsync(request.Id, cancellationToken);
        if (employee is null) return Result<EmployeeDto>.Failure("Employee not found.");
        return Result<EmployeeDto>.Success(_mapper.Map<EmployeeDto>(employee));
    }
}
