using Dapper;
using Npgsql;
using TournamentApp.Application.Interfaces;
using TournamentApp.Domain.Entities;

namespace TournamentApp.Infrastructure.Data;

public class PlayerRepository : IPlayerRepository
{
    private readonly string _connectionString;

    public PlayerRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<Guid> CreateAsync(Player player)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var sql = @"
            INSERT INTO ""Players"" (""Id"", ""Name"", ""CreatedAt"")
            VALUES (@Id, @Name, @CreatedAt)";

        await connection.ExecuteAsync(sql, new
        {
            player.Id,
            player.Name,
            player.CreatedAt
        });

        return player.Id;
    }

    public async Task<Player?> GetByIdAsync(Guid id)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var sql = @"
            SELECT ""Id"", ""Name"", ""CreatedAt""
            FROM ""Players""
            WHERE ""Id"" = @Id";

        var result = await connection.QueryFirstOrDefaultAsync<Player>(sql, new { Id = id });
        return result;
    }

    public async Task<List<Player>> GetAllAsync()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var sql = @"
            SELECT ""Id"", ""Name"", ""CreatedAt""
            FROM ""Players""
            ORDER BY ""Name""";

        var results = await connection.QueryAsync<Player>(sql);
        return results.ToList();
    }
}

