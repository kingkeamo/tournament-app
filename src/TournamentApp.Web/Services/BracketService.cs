using TournamentApp.Shared;
using TournamentApp.Web.Contracts.Services;
using TournamentApp.Web.Extensions;
using TournamentApp.Web.Responses;

namespace TournamentApp.Web.Services;

public class BracketService : IBracketService
{
    private readonly HttpClient _httpClient;

    public BracketService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Response> GenerateBracket(Guid tournamentId)
    {
        var httpResponse = await _httpClient.PostAsync($"api/tournaments/{tournamentId}/bracket/generate", null);
        return await httpResponse.GetResponseData<Response>();
    }

    public async Task<DataResponse<BracketDto>> GetBracket(Guid tournamentId)
    {
        var httpResponse = await _httpClient.GetAsync($"api/tournaments/{tournamentId}/bracket");
        return await httpResponse.GetResponseData<DataResponse<BracketDto>>();
    }
}





