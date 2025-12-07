namespace TournamentApp.Domain.Entities;

public class Match
{
    public Guid Id { get; set; }
    public Guid TournamentId { get; set; }
    public int Round { get; set; }
    public int Position { get; set; }
    public Guid? Player1Id { get; set; }
    public Guid? Player2Id { get; set; }
    public int Score1 { get; set; }
    public int Score2 { get; set; }
    public Guid? WinnerId { get; set; }
    public MatchStatus Status { get; set; } = MatchStatus.Pending;
    public DateTime CreatedAt { get; set; }
}

public enum MatchStatus
{
    Pending,
    InProgress,
    Completed,
    Bye
}

