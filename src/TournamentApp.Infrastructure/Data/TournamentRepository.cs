using Dapper;
using Npgsql;
using TournamentApp.Application.Interfaces;
using TournamentApp.Domain.Entities;

namespace TournamentApp.Infrastructure.Data;

public class TournamentRepository : ITournamentRepository
{
    private readonly string _connectionString;

    public TournamentRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<Guid> CreateAsync(Tournament tournament)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var sql = @"
            INSERT INTO ""Tournaments"" (""Id"", ""Name"", ""Status"", ""CreatedAt"")
            VALUES (@Id, @Name, @Status, @CreatedAt)";

        await connection.ExecuteAsync(sql, new
        {
            tournament.Id,
            tournament.Name,
            Status = tournament.Status.ToString(),
            tournament.CreatedAt
        });

        return tournament.Id;
    }

    public async Task<Tournament?> GetByIdAsync(Guid id)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var sql = @"
            SELECT ""Id"", ""Name"", ""Status"", ""CreatedAt""
            FROM ""Tournaments""
            WHERE ""Id"" = @Id";

        var result = await connection.QueryFirstOrDefaultAsync<dynamic>(sql, new { Id = id });
        if (result == null) return null;

        var tournament = new Tournament
        {
            Id = result.Id,
            Name = result.Name,
            Status = Enum.Parse<TournamentStatus>(result.Status),
            CreatedAt = result.CreatedAt
        };

        tournament.PlayerIds = await GetPlayerIdsAsync(id);
        return tournament;
    }

    public async Task<List<Tournament>> GetAllAsync()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var sql = @"
            SELECT ""Id"", ""Name"", ""Status"", ""CreatedAt""
            FROM ""Tournaments""
            ORDER BY ""CreatedAt"" DESC";

        var results = await connection.QueryAsync<dynamic>(sql);
        var tournaments = new List<Tournament>();

        foreach (var result in results)
        {
            var tournament = new Tournament
            {
                Id = result.Id,
                Name = result.Name,
                Status = Enum.Parse<TournamentStatus>(result.Status),
                CreatedAt = result.CreatedAt
            };

            tournament.PlayerIds = await GetPlayerIdsAsync(tournament.Id);
            tournaments.Add(tournament);
        }

        return tournaments;
    }

    public async Task AddPlayerAsync(Guid tournamentId, Guid playerId)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var sql = @"
            INSERT INTO ""TournamentPlayers"" (""TournamentId"", ""PlayerId"")
            VALUES (@TournamentId, @PlayerId)
            ON CONFLICT DO NOTHING";

        await connection.ExecuteAsync(sql, new { TournamentId = tournamentId, PlayerId = playerId });
    }

    public async Task RemovePlayerAsync(Guid tournamentId, Guid playerId)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var sql = @"
            DELETE FROM ""TournamentPlayers""
            WHERE ""TournamentId"" = @TournamentId AND ""PlayerId"" = @PlayerId";

        await connection.ExecuteAsync(sql, new { TournamentId = tournamentId, PlayerId = playerId });
    }

    public async Task<List<Guid>> GetPlayerIdsAsync(Guid tournamentId)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var sql = @"
            SELECT ""PlayerId""
            FROM ""TournamentPlayers""
            WHERE ""TournamentId"" = @TournamentId";

        var playerIds = await connection.QueryAsync<Guid>(sql, new { TournamentId = tournamentId });
        return playerIds.ToList();
    }
}

