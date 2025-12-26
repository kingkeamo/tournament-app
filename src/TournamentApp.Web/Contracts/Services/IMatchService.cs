using TournamentApp.Web.Responses;

namespace TournamentApp.Web.Contracts.Services;

public interface IMatchService
{
    Task<Response> UpdateMatchScore(Guid matchId, UpdateMatchScoreViewModel viewModel);
}

public class UpdateMatchScoreViewModel
{
    public Guid MatchId { get; set; }
    public int Score1 { get; set; }
    public int Score2 { get; set; }
}





