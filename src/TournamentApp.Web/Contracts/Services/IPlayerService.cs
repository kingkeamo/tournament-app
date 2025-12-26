using TournamentApp.Shared;
using TournamentApp.Web.Responses;

namespace TournamentApp.Web.Contracts.Services;

public interface IPlayerService
{
    Task<DataResponse<IEnumerable<PlayerDto>>> GetPlayers();
    Task<CreateResponse> AddPlayer(AddPlayerViewModel viewModel);
}

public class AddPlayerViewModel
{
    public string Name { get; set; } = string.Empty;
}





