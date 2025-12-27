using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TournamentApp.Application.Players.Commands;
using TournamentApp.Application.Players.Queries;

namespace TournamentApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize] // Commented out until authentication is configured
public class PlayersController : ControllerBase
{
    private readonly IMediator _mediator;

    public PlayersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePlayer([FromBody] AddPlayerCommand command)
    {
        var response = await _mediator.Send(command);
        if (response.IsFailure)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetPlayers()
    {
        var response = await _mediator.Send(new GetPlayersQuery());
        if (response.IsFailure)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}
