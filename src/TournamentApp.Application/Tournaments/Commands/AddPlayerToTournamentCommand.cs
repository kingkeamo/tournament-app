using FluentValidation;
using MediatR;
using TournamentApp.Application.Common.Responses;
using TournamentApp.Application.Interfaces;

namespace TournamentApp.Application.Tournaments.Commands;

public class AddPlayerToTournamentCommand : IRequest<AddPlayerToTournamentResponse>
{
    public Guid TournamentId { get; set; }
    public List<Guid> PlayerIds { get; set; } = new();
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

        if (request.PlayerIds == null || !request.PlayerIds.Any())
        {
            return new AddPlayerToTournamentResponse
            {
                ErrorMessage = "At least one player must be selected"
            };
        }

        // Get existing player IDs to check for duplicates
        var existingPlayerIds = await _tournamentRepository.GetPlayerIdsAsync(request.TournamentId);
        var duplicateIds = request.PlayerIds.Intersect(existingPlayerIds).ToList();
        
        // Filter out duplicates - only add players that aren't already in the tournament
        var newPlayerIds = request.PlayerIds.Except(existingPlayerIds).ToList();

        if (!newPlayerIds.Any())
        {
            var duplicateMessage = duplicateIds.Count == 1
                ? $"Player is already in this tournament"
                : $"{duplicateIds.Count} players are already in this tournament";
            return new AddPlayerToTournamentResponse
            {
                ErrorMessage = duplicateMessage
            };
        }

        // Validate all players exist
        var invalidPlayerIds = new List<Guid>();
        foreach (var playerId in newPlayerIds)
        {
            var player = await _playerRepository.GetByIdAsync(playerId);
            if (player == null)
            {
                invalidPlayerIds.Add(playerId);
            }
        }

        if (invalidPlayerIds.Any())
        {
            return new AddPlayerToTournamentResponse
            {
                ErrorMessage = $"One or more players not found"
            };
        }

        // Add all new players (duplicates are automatically skipped by ON CONFLICT DO NOTHING)
        await _tournamentRepository.AddPlayersAsync(request.TournamentId, newPlayerIds);

        // Build response message
        var addedCount = newPlayerIds.Count;
        var skippedCount = duplicateIds.Count;
        var response = new AddPlayerToTournamentResponse();
        
        if (skippedCount > 0)
        {
            response.ErrorMessage = $"Added {addedCount} player(s). {skippedCount} player(s) were already in the tournament.";
        }

        return response;
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
        RuleFor(x => x.PlayerIds)
            .NotNull().WithMessage("Player IDs are required")
            .NotEmpty().WithMessage("At least one player must be selected");
    }
}







