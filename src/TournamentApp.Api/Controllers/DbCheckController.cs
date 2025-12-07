using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace TournamentApp.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class DbCheckController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public DbCheckController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                return StatusCode(500, new { status = "error", message = "Connection string not configured" });
            }

            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();
            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT 1";
            await command.ExecuteScalarAsync();

            return Ok(new { status = "connected", timestamp = DateTime.UtcNow });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { status = "error", message = ex.Message });
        }
    }
}

