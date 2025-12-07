using MediatR;
using TournamentApp.Application.Commands;
using TournamentApp.Application.Interfaces;
using TournamentApp.Domain.Entities;
using TournamentApp.Domain.Services;

namespace TournamentApp.Application.Handlers;

public class UpdateMatchScoreHandler : IRequestHandler<UpdateMatchScoreCommand>
{
    private readonly IMatchRepository _matchRepository;
    private readonly ITournamentRepository _tournamentRepository;
    private readonly BracketGenerator _bracketGenerator;

    public UpdateMatchScoreHandler(
        IMatchRepository matchRepository,
        ITournamentRepository tournamentRepository,
        BracketGenerator bracketGenerator)
    {
        _matchRepository = matchRepository;
        _tournamentRepository = tournamentRepository;
        _bracketGenerator = bracketGenerator;
    }

    public async Task Handle(UpdateMatchScoreCommand request, CancellationToken cancellationToken)
    {
        var match = await _matchRepository.GetByIdAsync(request.MatchId);
        if (match == null)
        {
            throw new InvalidOperationException($"Match with ID {request.MatchId} not found.");
        }

        match.Score1 = request.Score1;
        match.Score2 = request.Score2;

        if (match.Score1 > match.Score2)
        {
            match.WinnerId = match.Player1Id;
        }
        else if (match.Score2 > match.Score1)
        {
            match.WinnerId = match.Player2Id;
        }
        else
        {
            throw new InvalidOperationException("Scores cannot be equal. A winner must be determined.");
        }

        match.Status = MatchStatus.Completed;

        await _matchRepository.UpdateAsync(match);

        // Advance winner to next round
        var allMatches = await _matchRepository.GetByTournamentIdAsync(match.TournamentId);
        _bracketGenerator.AdvanceWinner(match, allMatches);

        // Update the next round match if it was modified
        var nextRoundMatch = allMatches.FirstOrDefault(m => 
            m.TournamentId == match.TournamentId 
            && m.Round == match.Round + 1 
            && m.Position == match.Position / 2);

        if (nextRoundMatch != null)
        {
            await _matchRepository.UpdateAsync(nextRoundMatch);
        }
    }
}

