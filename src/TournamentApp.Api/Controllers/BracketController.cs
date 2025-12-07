using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TournamentApp.Application.Commands;
using TournamentApp.Application.DTOs;
using TournamentApp.Application.Queries;

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
        await _mediator.Send(new GenerateBracketCommand { TournamentId = tournamentId });
        return Ok();
    }

    [HttpGet]
    public async Task<ActionResult<BracketDto>> GetBracket(Guid tournamentId)
    {
        var bracket = await _mediator.Send(new GetBracketQuery { TournamentId = tournamentId });
        if (bracket == null)
        {
            return NotFound();
        }
        return Ok(bracket);
    }
}

