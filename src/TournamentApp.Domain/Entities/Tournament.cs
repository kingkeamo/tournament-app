namespace TournamentApp.Domain.Entities;

public class Tournament
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public TournamentStatus Status { get; set; } = TournamentStatus.Draft;
    public DateTime CreatedAt { get; set; }
    public List<Guid> PlayerIds { get; set; } = new();
}

public enum TournamentStatus
{
    Draft,
    InProgress,
    Completed
}

