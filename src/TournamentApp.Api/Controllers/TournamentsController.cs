using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TournamentApp.Application.Commands;
using TournamentApp.Application.Queries;
using TournamentApp.Shared;

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
    public async Task<ActionResult<Guid>> CreateTournament([FromBody] CreateTournamentCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(id);
    }

    [HttpGet]
    public async Task<ActionResult<List<TournamentDto>>> GetTournaments()
    {
        var tournaments = await _mediator.Send(new GetTournamentListQuery());
        return Ok(tournaments);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TournamentDto>> GetTournament(Guid id)
    {
        var tournament = await _mediator.Send(new GetTournamentQuery { TournamentId = id });
        if (tournament == null)
        {
            return NotFound();
        }
        return Ok(tournament);
    }
}

