using AutoMapper;
using ECommerce.Application.Common.Models;
using ECommerce.Domain.Interfaces;
using MediatR;

namespace ECommerce.Application.Features.Reviews.Queries;

public record GetAllReviewsQuery : IRequest<IReadOnlyList<ReviewDto>>;

public class GetAllReviewsQueryHandler : IRequestHandler<GetAllReviewsQuery, IReadOnlyList<ReviewDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllReviewsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ReviewDto>> Handle(GetAllReviewsQuery request, CancellationToken cancellationToken)
    {
        var reviews = await _unitOfWork.Reviews.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<ReviewDto>>(reviews);
    }
}

public record GetReviewByIdQuery(Guid Id) : IRequest<Result<ReviewDto>>;

public class GetReviewByIdQueryHandler : IRequestHandler<GetReviewByIdQuery, Result<ReviewDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetReviewByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<ReviewDto>> Handle(GetReviewByIdQuery request, CancellationToken cancellationToken)
    {
        var review = await _unitOfWork.Reviews.GetByIdAsync(request.Id, cancellationToken);
        if (review is null) return Result<ReviewDto>.Failure("Review not found.");
        return Result<ReviewDto>.Success(_mapper.Map<ReviewDto>(review));
    }
}

public record GetReviewsByProductQuery(Guid ProductId) : IRequest<IReadOnlyList<ReviewDto>>;

public class GetReviewsByProductQueryHandler : IRequestHandler<GetReviewsByProductQuery, IReadOnlyList<ReviewDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetReviewsByProductQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ReviewDto>> Handle(GetReviewsByProductQuery request, CancellationToken cancellationToken)
    {
        var reviews = await _unitOfWork.Reviews.GetByProductIdAsync(request.ProductId, cancellationToken);
        return _mapper.Map<IReadOnlyList<ReviewDto>>(reviews);
    }
}
