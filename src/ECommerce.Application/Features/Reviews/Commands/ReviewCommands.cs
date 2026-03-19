using ECommerce.Application.Common.Models;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace ECommerce.Application.Features.Reviews.Commands;

public record CreateReviewCommand(
    Guid ProductId, Guid CustomerId, int Rating, string Title, string Comment
) : IRequest<Result<Guid>>;

public class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
{
    public CreateReviewCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.Rating).InclusiveBetween(1, 5);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Comment).MaximumLength(2000);
    }
}

public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateReviewCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Result<Guid>> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null) return Result<Guid>.Failure("Product not found.");

        var customer = await _unitOfWork.Customers.GetByIdAsync(request.CustomerId, cancellationToken);
        if (customer is null) return Result<Guid>.Failure("Customer not found.");

        var review = Review.Create(request.ProductId, request.CustomerId, request.Rating, request.Title, request.Comment);

        await _unitOfWork.Reviews.AddAsync(review, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(review.Id);
    }
}

public record ApproveReviewCommand(Guid Id) : IRequest<Result<bool>>;

public class ApproveReviewCommandHandler : IRequestHandler<ApproveReviewCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ApproveReviewCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Result<bool>> Handle(ApproveReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await _unitOfWork.Reviews.GetByIdAsync(request.Id, cancellationToken);
        if (review is null) return Result<bool>.Failure("Review not found.");

        review.Approve();
        await _unitOfWork.Reviews.UpdateAsync(review, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}

public record DeleteReviewCommand(Guid Id) : IRequest<Result<bool>>;

public class DeleteReviewCommandHandler : IRequestHandler<DeleteReviewCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteReviewCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Result<bool>> Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await _unitOfWork.Reviews.GetByIdAsync(request.Id, cancellationToken);
        if (review is null) return Result<bool>.Failure("Review not found.");

        await _unitOfWork.Reviews.DeleteAsync(review, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
