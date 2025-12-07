namespace TournamentApp.Application.DTOs;

public class TournamentDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<Guid> PlayerIds { get; set; } = new();
}

