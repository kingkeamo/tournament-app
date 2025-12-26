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
    public string Name { get; set; } = string.Empty;
}

public class AddPlayerToTournamentViewModel
{
    public Guid TournamentId { get; set; }
    public Guid PlayerId { get; set; }
}





