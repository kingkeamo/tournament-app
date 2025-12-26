using FluentValidation;
using MediatR;
using TournamentApp.Application.Common.Responses;
using TournamentApp.Application.Interfaces;

namespace TournamentApp.Application.Tournaments.Commands;

public class AddPlayerToTournamentCommand : IRequest<AddPlayerToTournamentResponse>
{
    public Guid TournamentId { get; set; }
    public Guid PlayerId { get; set; }
}

public class AddPlayerToTournamentHandler : IRequestHandler<AddPlayerToTournamentCommand, AddPlayerToTournamentResponse>
{
    private readonly ITournamentRepository _tournamentRepository;
    private readonly IPlayerRepository _playerRepository;

    public AddPlayerToTournamentHandler(
        ITournamentRepository tournamentRepository,
        IPlayerRepository playerRepository)
    {
        _tournamentRepository = tournamentRepository;
        _playerRepository = playerRepository;
    }

    public async Task<AddPlayerToTournamentResponse> Handle(AddPlayerToTournamentCommand request, CancellationToken cancellationToken)
    {
        // Business rule: Tournament must exist
        var tournament = await _tournamentRepository.GetByIdAsync(request.TournamentId);
        if (tournament == null)
        {
            return new AddPlayerToTournamentResponse
            {
                ErrorMessage = $"Tournament with ID {request.TournamentId} not found"
            };
        }

        // Business rule: Player must exist
        var player = await _playerRepository.GetByIdAsync(request.PlayerId);
        if (player == null)
        {
            return new AddPlayerToTournamentResponse
            {
                ErrorMessage = $"Player with ID {request.PlayerId} not found"
            };
        }

        await _tournamentRepository.AddPlayerAsync(request.TournamentId, request.PlayerId);

        return new AddPlayerToTournamentResponse();
    }
}

public class AddPlayerToTournamentResponse : ValidatedResponse
{
}

public class AddPlayerToTournamentCommandValidator : AbstractValidator<AddPlayerToTournamentCommand>
{
    public AddPlayerToTournamentCommandValidator()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.TournamentId).NotEmpty().WithMessage("Tournament ID is required");
        RuleFor(x => x.PlayerId).NotEmpty().WithMessage("Player ID is required");
    }
}







