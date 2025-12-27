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
    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Player name is required")]
    [System.ComponentModel.DataAnnotations.MaxLength(255, ErrorMessage = "Player name cannot exceed 255 characters")]
    public string Name { get; set; } = string.Empty;
}





