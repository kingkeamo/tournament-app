using TournamentApp.Domain.Entities;

namespace TournamentApp.Application.Interfaces;

public interface IMatchRepository
{
    Task CreateAsync(Match match);
    Task CreateManyAsync(List<Match> matches);
    Task<Match?> GetByIdAsync(Guid id);
    Task<List<Match>> GetByTournamentIdAsync(Guid tournamentId);
    Task UpdateAsync(Match match);
}

