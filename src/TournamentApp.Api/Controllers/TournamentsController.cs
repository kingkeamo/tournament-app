using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TournamentApp.Application.Tournaments.Commands;
using TournamentApp.Application.Tournaments.Queries;

namespace TournamentApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize] // Commented out until authentication is configured
public class TournamentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TournamentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTournament([FromBody] CreateTournamentCommand command)
    {
        var response = await _mediator.Send(command);
        if (response.IsFailure)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetTournaments()
    {
        var response = await _mediator.Send(new GetTournamentListQuery());
        if (response.IsFailure)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTournament(Guid id)
    {
        var response = await _mediator.Send(new GetTournamentQuery { TournamentId = id });
        if (response.IsFailure)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPost("{id}/players")]
    public async Task<IActionResult> AddPlayerToTournament(Guid id, [FromBody] AddPlayerToTournamentCommand command)
    {
        if (id != command.TournamentId)
        {
            return BadRequest("Tournament ID mismatch");
        }

        var response = await _mediator.Send(command);
        if (response.IsFailure)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpDelete("{id}/players/{playerId}")]
    public async Task<IActionResult> RemovePlayerFromTournament(Guid id, Guid playerId)
    {
        var command = new RemovePlayerFromTournamentCommand 
        { 
            TournamentId = id, 
            PlayerId = playerId 
        };
        
        var response = await _mediator.Send(command);
        if (response.IsFailure)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}
