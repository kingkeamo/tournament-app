using FluentValidation;
using MediatR;
using TournamentApp.Application.Common.Responses;
using TournamentApp.Application.Interfaces;

namespace TournamentApp.Application.Tournaments.Commands;

public class RemovePlayerFromTournamentCommand : IRequest<RemovePlayerFromTournamentResponse>
{
    public Guid TournamentId { get; set; }
    public Guid PlayerId { get; set; }
}

public class RemovePlayerFromTournamentHandler : IRequestHandler<RemovePlayerFromTournamentCommand, RemovePlayerFromTournamentResponse>
{
    private readonly ITournamentRepository _tournamentRepository;
    private readonly IPlayerRepository _playerRepository;

    public RemovePlayerFromTournamentHandler(
        ITournamentRepository tournamentRepository,
        IPlayerRepository playerRepository)
    {
        _tournamentRepository = tournamentRepository;
        _playerRepository = playerRepository;
    }

    public async Task<RemovePlayerFromTournamentResponse> Handle(RemovePlayerFromTournamentCommand request, CancellationToken cancellationToken)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(request.TournamentId);
        if (tournament == null)
        {
            return new RemovePlayerFromTournamentResponse
            {
                ErrorMessage = $"Tournament with ID {request.TournamentId} not found"
            };
        }

        var player = await _playerRepository.GetByIdAsync(request.PlayerId);
        if (player == null)
        {
            return new RemovePlayerFromTournamentResponse
            {
                ErrorMessage = $"Player with ID {request.PlayerId} not found"
            };
        }

        var existingPlayerIds = await _tournamentRepository.GetPlayerIdsAsync(request.TournamentId);
        if (!existingPlayerIds.Contains(request.PlayerId))
        {
            return new RemovePlayerFromTournamentResponse
            {
                ErrorMessage = $"Player '{player.Name}' is not in this tournament"
            };
        }

        await _tournamentRepository.RemovePlayerAsync(request.TournamentId, request.PlayerId);

        return new RemovePlayerFromTournamentResponse();
    }
}

public class RemovePlayerFromTournamentResponse : ValidatedResponse
{
}

public class RemovePlayerFromTournamentCommandValidator : AbstractValidator<RemovePlayerFromTournamentCommand>
{
    public RemovePlayerFromTournamentCommandValidator()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.TournamentId).NotEmpty().WithMessage("Tournament ID is required");
        RuleFor(x => x.PlayerId).NotEmpty().WithMessage("Player ID is required");
    }
}

