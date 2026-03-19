using ECommerce.Application.Common.Models;
using ECommerce.Domain.Interfaces;
using MediatR;

namespace ECommerce.Application.Features.Categories.Commands;

public record ToggleCategoryStatusCommand(Guid Id) : IRequest<Result<bool>>;

public class ToggleCategoryStatusCommandHandler : IRequestHandler<ToggleCategoryStatusCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ToggleCategoryStatusCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(ToggleCategoryStatusCommand request, CancellationToken cancellationToken)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(request.Id, cancellationToken);
        if (category is null)
            return Result<bool>.Failure("Category not found.");

        if (category.IsActive)
            category.Deactivate();
        else
            category.Activate();

        await _unitOfWork.Categories.UpdateAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(category.IsActive);
    }
}
