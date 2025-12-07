using MediatR;
using TournamentApp.Application.DTOs;
using TournamentApp.Application.Interfaces;
using TournamentApp.Application.Queries;

namespace TournamentApp.Application.Handlers;

public class GetTournamentHandler : IRequestHandler<GetTournamentQuery, TournamentDto?>
{
    private readonly ITournamentRepository _repository;

    public GetTournamentHandler(ITournamentRepository repository)
    {
        _repository = repository;
    }

    public async Task<TournamentDto?> Handle(GetTournamentQuery request, CancellationToken cancellationToken)
    {
        var tournament = await _repository.GetByIdAsync(request.TournamentId);
        if (tournament == null)
        {
            return null;
        }

        return new TournamentDto
        {
            Id = tournament.Id,
            Name = tournament.Name,
            Status = tournament.Status.ToString(),
            CreatedAt = tournament.CreatedAt,
            PlayerIds = tournament.PlayerIds
        };
    }
}

