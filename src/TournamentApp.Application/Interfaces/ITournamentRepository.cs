using TournamentApp.Domain.Entities;

namespace TournamentApp.Application.Interfaces;

public interface ITournamentRepository
{
    Task<Guid> CreateAsync(Tournament tournament);
    Task<Tournament?> GetByIdAsync(Guid id);
    Task<List<Tournament>> GetAllAsync();
    Task AddPlayerAsync(Guid tournamentId, Guid playerId);
    Task AddPlayersAsync(Guid tournamentId, List<Guid> playerIds);
    Task RemovePlayerAsync(Guid tournamentId, Guid playerId);
    Task<List<Guid>> GetPlayerIdsAsync(Guid tournamentId);
    Task UpdateStatusAsync(Guid tournamentId, TournamentStatus status);
}

