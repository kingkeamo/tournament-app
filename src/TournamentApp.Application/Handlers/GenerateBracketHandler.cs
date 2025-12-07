using MediatR;
using TournamentApp.Application.Commands;
using TournamentApp.Application.Interfaces;
using TournamentApp.Domain.Services;

namespace TournamentApp.Application.Handlers;

public class GenerateBracketHandler : IRequestHandler<GenerateBracketCommand>
{
    private readonly ITournamentRepository _tournamentRepository;
    private readonly IMatchRepository _matchRepository;
    private readonly BracketGenerator _bracketGenerator;

    public GenerateBracketHandler(
        ITournamentRepository tournamentRepository,
        IMatchRepository matchRepository,
        BracketGenerator bracketGenerator)
    {
        _tournamentRepository = tournamentRepository;
        _matchRepository = matchRepository;
        _bracketGenerator = bracketGenerator;
    }

    public async Task Handle(GenerateBracketCommand request, CancellationToken cancellationToken)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(request.TournamentId);
        if (tournament == null)
        {
            throw new InvalidOperationException($"Tournament with ID {request.TournamentId} not found.");
        }

        var playerIds = await _tournamentRepository.GetPlayerIdsAsync(request.TournamentId);
        if (playerIds.Count < 2)
        {
            throw new InvalidOperationException("At least 2 players are required to generate a bracket.");
        }

        var matches = _bracketGenerator.GenerateSingleEliminationBracket(request.TournamentId, playerIds);
        await _matchRepository.CreateManyAsync(matches);
    }
}

