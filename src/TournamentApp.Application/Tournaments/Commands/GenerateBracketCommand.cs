using FluentValidation;
using MediatR;
using TournamentApp.Application.Common.Responses;
using TournamentApp.Application.Interfaces;
using TournamentApp.Domain.Services;

namespace TournamentApp.Application.Tournaments.Commands;

public class GenerateBracketCommand : IRequest<GenerateBracketResponse>
{
    public Guid TournamentId { get; set; }
}

public class GenerateBracketHandler : IRequestHandler<GenerateBracketCommand, GenerateBracketResponse>
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

    public async Task<GenerateBracketResponse> Handle(GenerateBracketCommand request, CancellationToken cancellationToken)
    {
        // Business rule: Tournament must exist
        var tournament = await _tournamentRepository.GetByIdAsync(request.TournamentId);
        if (tournament == null)
        {
            return new GenerateBracketResponse
            {
                ErrorMessage = $"Tournament with ID {request.TournamentId} not found"
            };
        }

        // Business rule: Tournament must have at least 2 players
        var playerIds = await _tournamentRepository.GetPlayerIdsAsync(request.TournamentId);
        if (playerIds.Count < 2)
        {
            return new GenerateBracketResponse
            {
                ErrorMessage = "At least 2 players are required to generate a bracket"
            };
        }

        var matches = _bracketGenerator.GenerateSingleEliminationBracket(request.TournamentId, playerIds);
        await _matchRepository.CreateManyAsync(matches);

        return new GenerateBracketResponse();
    }
}

public class GenerateBracketResponse : ValidatedResponse
{
}

public class GenerateBracketCommandValidator : AbstractValidator<GenerateBracketCommand>
{
    public GenerateBracketCommandValidator()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.TournamentId).NotEmpty().WithMessage("Tournament ID is required");
    }
}



