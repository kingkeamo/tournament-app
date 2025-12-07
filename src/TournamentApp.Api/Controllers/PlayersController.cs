using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TournamentApp.Application.Commands;
using TournamentApp.Application.DTOs;
using TournamentApp.Application.Queries;

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
    public async Task<ActionResult<Guid>> CreatePlayer([FromBody] AddPlayerCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(id);
    }

    [HttpGet]
    public async Task<ActionResult<List<PlayerDto>>> GetPlayers()
    {
        var players = await _mediator.Send(new GetPlayersQuery());
        return Ok(players);
    }
}

