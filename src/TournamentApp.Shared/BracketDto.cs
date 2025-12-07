namespace TournamentApp.Shared;

public class BracketDto
{
    public Guid TournamentId { get; set; }
    public List<MatchDto> Matches { get; set; } = new();
    public Dictionary<int, List<MatchDto>> MatchesByRound { get; set; } = new();
}

