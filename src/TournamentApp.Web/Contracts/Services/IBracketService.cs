using TournamentApp.Shared;
using TournamentApp.Web.Responses;

namespace TournamentApp.Web.Contracts.Services;

public interface IBracketService
{
    Task<Response> GenerateBracket(Guid tournamentId);
    Task<DataResponse<BracketDto>> GetBracket(Guid tournamentId);
}





