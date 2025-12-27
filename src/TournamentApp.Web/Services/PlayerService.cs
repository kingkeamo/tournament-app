using System.Net.Http.Json;
using TournamentApp.Shared;
using TournamentApp.Web.Contracts.Services;
using TournamentApp.Web.Extensions;
using TournamentApp.Web.Responses;

namespace TournamentApp.Web.Services;

public class PlayerService : IPlayerService
{
    private readonly HttpClient _httpClient;

    public PlayerService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<DataResponse<IEnumerable<PlayerDto>>> GetPlayers()
    {
        var httpResponse = await _httpClient.GetAsync("api/players");
        return await httpResponse.GetResponseData<DataResponse<IEnumerable<PlayerDto>>>();
    }

    public async Task<CreateResponse> AddPlayer(AddPlayerViewModel viewModel)
    {
        var httpResponse = await _httpClient.PostAsJsonAsync("api/players", viewModel);
        return await httpResponse.GetResponseData<CreateResponse>();
    }
}





