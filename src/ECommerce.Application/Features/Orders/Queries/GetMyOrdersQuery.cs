using AutoMapper;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Interfaces;
using MediatR;

namespace ECommerce.Application.Features.Orders.Queries;

public record GetMyOrdersQuery : IRequest<IReadOnlyList<OrderDto>>;

public class GetMyOrdersQueryHandler : IRequestHandler<GetMyOrdersQuery, IReadOnlyList<OrderDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public GetMyOrdersQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<OrderDto>> Handle(GetMyOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _unitOfWork.Orders.GetByCustomerIdAsync(_currentUser.UserId!, cancellationToken);
        return _mapper.Map<IReadOnlyList<OrderDto>>(orders);
    }
}
