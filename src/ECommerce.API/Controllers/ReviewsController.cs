using ECommerce.Application.Features.Reviews.Commands;
using ECommerce.Application.Features.Reviews.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReviewsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ReviewDto>>> GetAll()
    {
        var reviews = await _mediator.Send(new GetAllReviewsQuery());
        return Ok(reviews);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ReviewDto>> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetReviewByIdQuery(id));
        return result.IsSuccess ? Ok(result.Data) : NotFound(result.Error);
    }

    [HttpGet("product/{productId:guid}")]
    public async Task<ActionResult<IReadOnlyList<ReviewDto>>> GetByProduct(Guid productId)
    {
        var reviews = await _mediator.Send(new GetReviewsByProductQuery(productId));
        return Ok(reviews);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateReviewCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Data }, result.Data)
            : BadRequest(result.Error);
    }

    [Authorize]
    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> Approve(Guid id)
    {
        var result = await _mediator.Send(new ApproveReviewCommand(id));
        return result.IsSuccess ? Ok() : NotFound(result.Error);
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteReviewCommand(id));
        return result.IsSuccess ? NoContent() : NotFound(result.Error);
    }
}
