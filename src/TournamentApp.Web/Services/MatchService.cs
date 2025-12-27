using System.Net.Http.Json;
using TournamentApp.Web.Contracts.Services;
using TournamentApp.Web.Extensions;
using TournamentApp.Web.Responses;

namespace TournamentApp.Web.Services;

public class MatchService : IMatchService
{
    private readonly HttpClient _httpClient;

    public MatchService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Response> UpdateMatchScore(Guid matchId, UpdateMatchScoreViewModel viewModel)
    {
        var httpResponse = await _httpClient.PostAsJsonAsync($"api/matches/{matchId}/score", viewModel);
        return await httpResponse.GetResponseData<Response>();
    }
}





