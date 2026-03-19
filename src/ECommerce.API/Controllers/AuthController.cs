using ECommerce.Application.Features.Auth.Commands;
using ECommerce.Application.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<ActionResult<string>> Register([FromBody] RegisterCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(new { Token = result.Data }) : BadRequest(new { Error = result.Error });
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login([FromBody] LoginCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(new { Token = result.Data }) : Unauthorized(new { Error = result.Error });
    }
}
