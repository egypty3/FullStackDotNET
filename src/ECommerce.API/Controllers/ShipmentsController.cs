using ECommerce.Application.Features.Shipments.Commands;
using ECommerce.Application.Features.Shipments.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ShipmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ShipmentsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ShipmentDto>>> GetAll()
    {
        var shipments = await _mediator.Send(new GetAllShipmentsQuery());
        return Ok(shipments);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ShipmentDto>> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetShipmentByIdQuery(id));
        return result.IsSuccess ? Ok(result.Data) : NotFound(result.Error);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateShipmentCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Data }, result.Data)
            : BadRequest(result.Error);
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateShipmentStatusRequest request)
    {
        var result = await _mediator.Send(new UpdateShipmentStatusCommand(id, request.Status));
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
}

public record UpdateShipmentStatusRequest(string Status);
