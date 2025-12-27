using TournamentApp.Web.Responses;

namespace TournamentApp.Web.Contracts.Services;

public interface IMatchService
{
    Task<Response> UpdateMatchScore(Guid matchId, UpdateMatchScoreViewModel viewModel);
}

public class UpdateMatchScoreViewModel
{
    public Guid MatchId { get; set; }
    
    [System.ComponentModel.DataAnnotations.Range(0, int.MaxValue, ErrorMessage = "Score must be 0 or greater")]
    public int Score1 { get; set; }
    
    [System.ComponentModel.DataAnnotations.Range(0, int.MaxValue, ErrorMessage = "Score must be 0 or greater")]
    public int Score2 { get; set; }
}





