using DbUp;
using DbUp.Engine;
using DbUp.Postgresql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Reflection;

namespace TournamentApp.Infrastructure.DbUp;

public class DatabaseMigrator
{
    private readonly string _connectionString;
    private readonly ILogger<DatabaseMigrator> _logger;

    public DatabaseMigrator(IConfiguration configuration, ILogger<DatabaseMigrator> logger)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        _logger = logger;
    }

    public DatabaseUpgradeResult Migrate()
    {
        _logger.LogInformation("Starting database migration...");

        // Parse URI and rebuild connection string properly
        string cleanConnectionString;
        try
        {
            var uri = new Uri(_connectionString);
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = uri.Host,
                Port = uri.Port == -1 ? 5432 : uri.Port,
                Database = uri.AbsolutePath.TrimStart('/'),
                Username = uri.UserInfo.Split(':')[0],
                Password = uri.UserInfo.Contains(':') ? uri.UserInfo.Split(':')[1] : string.Empty,
                SslMode = Npgsql.SslMode.Require
            };
            
            cleanConnectionString = builder.ConnectionString;
            _logger.LogInformation("Connection string rebuilt from URI components");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to parse as URI, using original connection string");
            // Fallback: just remove channel_binding
            cleanConnectionString = System.Text.RegularExpressions.Regex.Replace(
                _connectionString,
                @"[&?]channel_binding=[^&\s]*",
                string.Empty,
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }

        // Connect directly to the database (Neon databases are already created)
        var upgrader = DeployChanges.To
            .PostgresqlDatabase(cleanConnectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), script => script.Contains(".DbUp.") && script.EndsWith(".sql"))
            .WithTransaction()
            .LogToConsole()
            .Build();

        var result = upgrader.PerformUpgrade();

        if (result.Successful)
        {
            _logger.LogInformation("Database migration completed successfully.");
        }
        else
        {
            _logger.LogError(result.Error, "Database migration failed.");
        }

        return result;
    }
}

