using TournamentApp.Domain.Entities;

namespace TournamentApp.Application.Interfaces;

public interface IPlayerRepository
{
    Task<Guid> CreateAsync(Player player);
    Task<Player?> GetByIdAsync(Guid id);
    Task<List<Player>> GetAllAsync();
}

