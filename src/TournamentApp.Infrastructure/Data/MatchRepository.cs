using Dapper;
using Npgsql;
using TournamentApp.Application.Interfaces;
using TournamentApp.Domain.Entities;

namespace TournamentApp.Infrastructure.Data;

public class MatchRepository : IMatchRepository
{
    private readonly string _connectionString;

    public MatchRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task CreateAsync(Match match)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var sql = @"
            INSERT INTO ""Matches"" (""Id"", ""TournamentId"", ""Round"", ""Position"", ""Player1Id"", ""Player2Id"", 
                ""Score1"", ""Score2"", ""WinnerId"", ""Status"", ""CreatedAt"")
            VALUES (@Id, @TournamentId, @Round, @Position, @Player1Id, @Player2Id, 
                @Score1, @Score2, @WinnerId, @Status, @CreatedAt)";

        await connection.ExecuteAsync(sql, new
        {
            match.Id,
            match.TournamentId,
            match.Round,
            match.Position,
            match.Player1Id,
            match.Player2Id,
            match.Score1,
            match.Score2,
            match.WinnerId,
            Status = match.Status.ToString(),
            match.CreatedAt
        });
    }

    public async Task CreateManyAsync(List<Match> matches)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var sql = @"
            INSERT INTO ""Matches"" (""Id"", ""TournamentId"", ""Round"", ""Position"", ""Player1Id"", ""Player2Id"", 
                ""Score1"", ""Score2"", ""WinnerId"", ""Status"", ""CreatedAt"")
            VALUES (@Id, @TournamentId, @Round, @Position, @Player1Id, @Player2Id, 
                @Score1, @Score2, @WinnerId, @Status, @CreatedAt)";

        var parameters = matches.Select(m => new
        {
            m.Id,
            m.TournamentId,
            m.Round,
            m.Position,
            m.Player1Id,
            m.Player2Id,
            m.Score1,
            m.Score2,
            m.WinnerId,
            Status = m.Status.ToString(),
            m.CreatedAt
        });

        await connection.ExecuteAsync(sql, parameters);
    }

    public async Task<Match?> GetByIdAsync(Guid id)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var sql = @"
            SELECT ""Id"", ""TournamentId"", ""Round"", ""Position"", ""Player1Id"", ""Player2Id"", 
                ""Score1"", ""Score2"", ""WinnerId"", ""Status"", ""CreatedAt""
            FROM ""Matches""
            WHERE ""Id"" = @Id";

        var result = await connection.QueryFirstOrDefaultAsync<dynamic>(sql, new { Id = id });
        if (result == null) return null;

        return new Match
        {
            Id = result.Id,
            TournamentId = result.TournamentId,
            Round = result.Round,
            Position = result.Position,
            Player1Id = result.Player1Id,
            Player2Id = result.Player2Id,
            Score1 = result.Score1,
            Score2 = result.Score2,
            WinnerId = result.WinnerId,
            Status = Enum.Parse<MatchStatus>(result.Status),
            CreatedAt = result.CreatedAt
        };
    }

    public async Task<List<Match>> GetByTournamentIdAsync(Guid tournamentId)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var sql = @"
            SELECT ""Id"", ""TournamentId"", ""Round"", ""Position"", ""Player1Id"", ""Player2Id"", 
                ""Score1"", ""Score2"", ""WinnerId"", ""Status"", ""CreatedAt""
            FROM ""Matches""
            WHERE ""TournamentId"" = @TournamentId
            ORDER BY ""Round"", ""Position""";

        var results = await connection.QueryAsync<dynamic>(sql, new { TournamentId = tournamentId });
        return results.Select(r => new Match
        {
            Id = r.Id,
            TournamentId = r.TournamentId,
            Round = r.Round,
            Position = r.Position,
            Player1Id = r.Player1Id,
            Player2Id = r.Player2Id,
            Score1 = r.Score1,
            Score2 = r.Score2,
            WinnerId = r.WinnerId,
            Status = Enum.Parse<MatchStatus>(r.Status),
            CreatedAt = r.CreatedAt
        }).ToList();
    }

    public async Task UpdateAsync(Match match)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var sql = @"
            UPDATE ""Matches""
            SET ""Round"" = @Round, ""Position"" = @Position, ""Player1Id"" = @Player1Id, ""Player2Id"" = @Player2Id,
                ""Score1"" = @Score1, ""Score2"" = @Score2, ""WinnerId"" = @WinnerId, ""Status"" = @Status
            WHERE ""Id"" = @Id";

        await connection.ExecuteAsync(sql, new
        {
            match.Id,
            match.Round,
            match.Position,
            match.Player1Id,
            match.Player2Id,
            match.Score1,
            match.Score2,
            match.WinnerId,
            Status = match.Status.ToString()
        });
    }
}

