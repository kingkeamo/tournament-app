using TournamentApp.Shared;
using TournamentApp.Web.Responses;

namespace TournamentApp.Web.Contracts.Services;

public interface ITournamentService
{
    Task<DataResponse<IEnumerable<TournamentDto>>> GetTournaments();
    Task<DataResponse<TournamentDto>> GetTournament(Guid tournamentId);
    Task<CreateResponse> CreateTournament(CreateTournamentViewModel viewModel);
    Task<Response> AddPlayerToTournament(Guid tournamentId, AddPlayerToTournamentViewModel viewModel);
}

public class CreateTournamentViewModel
{
    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Tournament name is required")]
    [System.ComponentModel.DataAnnotations.MaxLength(255, ErrorMessage = "Tournament name cannot exceed 255 characters")]
    public string Name { get; set; } = string.Empty;
}

public class AddPlayerToTournamentViewModel
{
    public Guid TournamentId { get; set; }
    
    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Please select a player")]
    public Guid PlayerId { get; set; }
}





