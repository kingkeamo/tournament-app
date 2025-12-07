namespace TournamentApp.Shared;

public class MatchDto
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
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

