using TournamentApp.Domain.Entities;

namespace TournamentApp.Domain.Services;

public class BracketGenerator
{
    public List<Match> GenerateSingleEliminationBracket(Guid tournamentId, List<Guid> playerIds)
    {
        if (playerIds.Count < 2)
        {
            throw new ArgumentException("At least 2 players are required to generate a bracket.", nameof(playerIds));
        }

        var matches = new List<Match>();
        var numberOfRounds = (int)Math.Ceiling(Math.Log2(playerIds.Count));
        var totalSlots = (int)Math.Pow(2, numberOfRounds);
        
        // Round 1: Create matches for all players
        var round1Matches = new List<Match>();
        var position = 0;

        for (int i = 0; i < playerIds.Count; i += 2)
        {
            var match = new Match
            {
                Id = Guid.NewGuid(),
                TournamentId = tournamentId,
                Round = 1,
                Position = position++,
                Player1Id = playerIds[i],
                Player2Id = i + 1 < playerIds.Count ? playerIds[i + 1] : null,
                Status = i + 1 < playerIds.Count ? MatchStatus.Pending : MatchStatus.Bye
            };

            if (match.Status == MatchStatus.Bye)
            {
                match.WinnerId = match.Player1Id;
            }

            round1Matches.Add(match);
        }

        matches.AddRange(round1Matches);

        // Generate subsequent rounds
        var currentRoundMatches = round1Matches;
        for (int round = 2; round <= numberOfRounds; round++)
        {
            var nextRoundMatches = new List<Match>();
            position = 0;

            for (int i = 0; i < currentRoundMatches.Count; i += 2)
            {
                var match = new Match
                {
                    Id = Guid.NewGuid(),
                    TournamentId = tournamentId,
                    Round = round,
                    Position = position++,
                    Status = MatchStatus.Pending
                };

                nextRoundMatches.Add(match);
            }

            matches.AddRange(nextRoundMatches);
            currentRoundMatches = nextRoundMatches;
        }

        return matches;
    }

    public void AdvanceWinner(Match match, List<Match> allMatches)
    {
        if (match.Status != MatchStatus.Completed || match.WinnerId == null)
        {
            throw new InvalidOperationException("Match must be completed with a winner to advance.");
        }

        var nextRoundMatch = allMatches
            .FirstOrDefault(m => m.TournamentId == match.TournamentId 
                && m.Round == match.Round + 1 
                && m.Position == match.Position / 2);

        if (nextRoundMatch == null)
        {
            return; // This is the final match
        }

        // Determine which player slot to fill based on position
        if (match.Position % 2 == 0)
        {
            nextRoundMatch.Player1Id = match.WinnerId;
        }
        else
        {
            nextRoundMatch.Player2Id = match.WinnerId;
        }

        // If both players are set, change status from Pending
        if (nextRoundMatch.Player1Id.HasValue && nextRoundMatch.Player2Id.HasValue)
        {
            nextRoundMatch.Status = MatchStatus.Pending;
        }
        else if (nextRoundMatch.Player1Id.HasValue || nextRoundMatch.Player2Id.HasValue)
        {
            // Only one player - this is a bye
            nextRoundMatch.Status = MatchStatus.Bye;
            nextRoundMatch.WinnerId = nextRoundMatch.Player1Id ?? nextRoundMatch.Player2Id;
        }
    }
}

