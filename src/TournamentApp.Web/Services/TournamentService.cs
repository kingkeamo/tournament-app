using System.Net.Http.Json;
using TournamentApp.Shared;
using TournamentApp.Web.Contracts.Services;
using TournamentApp.Web.Extensions;
using TournamentApp.Web.Responses;

namespace TournamentApp.Web.Services;

public class TournamentService : ITournamentService
{
    private readonly HttpClient _httpClient;

    public TournamentService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<DataResponse<IEnumerable<TournamentDto>>> GetTournaments()
    {
        var httpResponse = await _httpClient.GetAsync("api/tournaments");
        return await httpResponse.GetResponseData<DataResponse<IEnumerable<TournamentDto>>>();
    }

    public async Task<DataResponse<TournamentDto>> GetTournament(Guid tournamentId)
    {
        var httpResponse = await _httpClient.GetAsync($"api/tournaments/{tournamentId}");
        return await httpResponse.GetResponseData<DataResponse<TournamentDto>>();
    }

    public async Task<CreateResponse> CreateTournament(CreateTournamentViewModel viewModel)
    {
        var httpResponse = await _httpClient.PostAsJsonAsync("api/tournaments", viewModel);
        return await httpResponse.GetResponseData<CreateResponse>();
    }

    public async Task<Response> AddPlayerToTournament(Guid tournamentId, AddPlayerToTournamentViewModel viewModel)
    {
        var httpResponse = await _httpClient.PostAsJsonAsync($"api/tournaments/{tournamentId}/players", viewModel);
        return await httpResponse.GetResponseData<Response>();
    }

    public async Task<Response> RemovePlayerFromTournament(Guid tournamentId, Guid playerId)
    {
        var httpResponse = await _httpClient.DeleteAsync($"api/tournaments/{tournamentId}/players/{playerId}");
        return await httpResponse.GetResponseData<Response>();
    }
}





