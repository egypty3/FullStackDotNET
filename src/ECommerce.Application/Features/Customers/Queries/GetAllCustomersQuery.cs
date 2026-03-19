using AutoMapper;
using ECommerce.Application.Common.Models;
using ECommerce.Domain.Interfaces;
using MediatR;

namespace ECommerce.Application.Features.Customers.Queries;

public record GetAllCustomersQuery : IRequest<IReadOnlyList<CustomerDto>>;

public class GetAllCustomersQueryHandler : IRequestHandler<GetAllCustomersQuery, IReadOnlyList<CustomerDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllCustomersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<CustomerDto>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
    {
        var customers = await _unitOfWork.Customers.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<CustomerDto>>(customers);
    }
}

public record GetCustomerByIdQuery(Guid Id) : IRequest<Result<CustomerDto>>;

public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, Result<CustomerDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCustomerByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<CustomerDto>> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.Customers.GetByIdAsync(request.Id, cancellationToken);
        if (customer is null) return Result<CustomerDto>.Failure("Customer not found.");
        return Result<CustomerDto>.Success(_mapper.Map<CustomerDto>(customer));
    }
}
