using MediatR;

namespace TournamentApp.Application.Commands;

public class UpdateMatchScoreCommand : IRequest
{
    public Guid MatchId { get; set; }
    public int Score1 { get; set; }
    public int Score2 { get; set; }
}

