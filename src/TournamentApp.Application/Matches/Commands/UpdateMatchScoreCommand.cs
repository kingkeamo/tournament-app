using FluentValidation;
using MediatR;
using TournamentApp.Application.Common.Responses;
using TournamentApp.Application.Interfaces;
using TournamentApp.Domain.Entities;
using TournamentApp.Domain.Services;

namespace TournamentApp.Application.Matches.Commands;

public class UpdateMatchScoreCommand : IRequest<UpdateMatchScoreResponse>
{
    public Guid MatchId { get; set; }
    public int Score1 { get; set; }
    public int Score2 { get; set; }
}

public class UpdateMatchScoreHandler : IRequestHandler<UpdateMatchScoreCommand, UpdateMatchScoreResponse>
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

    public async Task<UpdateMatchScoreResponse> Handle(UpdateMatchScoreCommand request, CancellationToken cancellationToken)
    {
        // Business rule: Match must exist
        var match = await _matchRepository.GetByIdAsync(request.MatchId);
        if (match == null)
        {
            return new UpdateMatchScoreResponse
            {
                ErrorMessage = $"Match with ID {request.MatchId} not found"
            };
        }

        // Business rule: Scores cannot be equal (must have a winner)
        if (request.Score1 == request.Score2)
        {
            return new UpdateMatchScoreResponse
            {
                ErrorMessage = "Scores cannot be equal. A winner must be determined."
            };
        }

        match.Score1 = request.Score1;
        match.Score2 = request.Score2;

        if (match.Score1 > match.Score2)
        {
            match.WinnerId = match.Player1Id;
        }
        else
        {
            match.WinnerId = match.Player2Id;
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
            && m.Position == (match.Position + 1) / 2);

        if (nextRoundMatch != null)
        {
            await _matchRepository.UpdateAsync(nextRoundMatch);
        }
        else
        {
            // This is the final match - tournament is now completed
            var tournament = await _tournamentRepository.GetByIdAsync(match.TournamentId);
            if (tournament != null)
            {
                tournament.Status = TournamentStatus.Completed;
                await _tournamentRepository.UpdateStatusAsync(match.TournamentId, tournament.Status);
            }
        }

        return new UpdateMatchScoreResponse();
    }
}

public class UpdateMatchScoreResponse : ValidatedResponse
{
}

public class UpdateMatchScoreCommandValidator : AbstractValidator<UpdateMatchScoreCommand>
{
    public UpdateMatchScoreCommandValidator()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.MatchId).NotEmpty().WithMessage("Match ID is required");
        RuleFor(x => x.Score1).GreaterThanOrEqualTo(0).WithMessage("Score1 must be greater than or equal to 0");
        RuleFor(x => x.Score2).GreaterThanOrEqualTo(0).WithMessage("Score2 must be greater than or equal to 0");
    }
}







