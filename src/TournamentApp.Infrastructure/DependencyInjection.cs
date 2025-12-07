using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using TournamentApp.Application.Interfaces;
using TournamentApp.Domain.Services;
using TournamentApp.Infrastructure.Data;
using TournamentApp.Infrastructure.DbUp;

namespace TournamentApp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var rawConnectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        // Parse and rebuild connection string to handle URI format and ensure proper formatting
        var connectionString = ParseConnectionString(rawConnectionString);

        // Register repositories
        services.AddScoped<ITournamentRepository>(_ => new TournamentRepository(connectionString));
        services.AddScoped<IPlayerRepository>(_ => new PlayerRepository(connectionString));
        services.AddScoped<IMatchRepository>(_ => new MatchRepository(connectionString));

        // Register domain services
        services.AddScoped<BracketGenerator>();

        // Register DbUp migrator
        services.AddScoped<DatabaseMigrator>();

        return services;
    }

    private static string ParseConnectionString(string rawConnectionString)
    {
        try
        {
            // Try parsing as URI (postgresql:// format from Neon)
            if (rawConnectionString.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
            {
                var uri = new Uri(rawConnectionString);
                var builder = new NpgsqlConnectionStringBuilder
                {
                    Host = uri.Host,
                    Port = uri.Port == -1 ? 5432 : uri.Port,
                    Database = uri.AbsolutePath.TrimStart('/'),
                    Username = uri.UserInfo.Split(':')[0],
                    Password = uri.UserInfo.Contains(':') ? uri.UserInfo.Split(':')[1] : string.Empty,
                    SslMode = Npgsql.SslMode.Require
                };
                
                // Return in standard format: Server=...;Port=...;Database=...;User Id=...;Password=...;SslMode=Require;
                return builder.ConnectionString;
            }
            
            // If already in standard format, parse and return it
            var standardBuilder = new NpgsqlConnectionStringBuilder(rawConnectionString);
            return standardBuilder.ConnectionString;
        }
        catch (Exception ex)
        {
            // If parsing fails, throw a clear error
            throw new InvalidOperationException(
                $"Failed to parse connection string. Expected format: 'Server=host;Port=5432;Database=db;User Id=user;Password=pass;' or 'postgresql://user:pass@host/db'. Error: {ex.Message}", ex);
        }
    }
}

