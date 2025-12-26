using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TournamentApp.Application.Matches.Queries;
using TournamentApp.Application.Tournaments.Commands;

namespace TournamentApp.Api.Controllers;

[ApiController]
[Route("api/tournaments/{tournamentId}/[controller]")]
//[Authorize] // Commented out until authentication is configured
public class BracketController : ControllerBase
{
    private readonly IMediator _mediator;

    public BracketController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateBracket(Guid tournamentId)
    {
        var command = new GenerateBracketCommand { TournamentId = tournamentId };
        var response = await _mediator.Send(command);
        
        if (response.IsFailure)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetBracket(Guid tournamentId)
    {
        var response = await _mediator.Send(new GetBracketQuery { TournamentId = tournamentId });
        if (response.IsFailure)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}
