using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TournamentApp.Application.Matches.Commands;

namespace TournamentApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize] // Commented out until authentication is configured
public class MatchesController : ControllerBase
{
    private readonly IMediator _mediator;

    public MatchesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("{id}/score")]
    public async Task<IActionResult> UpdateScore(Guid id, [FromBody] UpdateMatchScoreCommand command)
    {
        if (id != command.MatchId)
        {
            return BadRequest("Match ID mismatch");
        }

        var response = await _mediator.Send(command);
        if (response.IsFailure)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}
