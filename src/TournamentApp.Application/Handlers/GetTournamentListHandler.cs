using MediatR;
using TournamentApp.Application.DTOs;
using TournamentApp.Application.Interfaces;
using TournamentApp.Application.Queries;

namespace TournamentApp.Application.Handlers;

public class GetTournamentListHandler : IRequestHandler<GetTournamentListQuery, List<TournamentDto>>
{
    private readonly ITournamentRepository _repository;

    public GetTournamentListHandler(ITournamentRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<TournamentDto>> Handle(GetTournamentListQuery request, CancellationToken cancellationToken)
    {
        var tournaments = await _repository.GetAllAsync();
        return tournaments.Select(t => new TournamentDto
        {
            Id = t.Id,
            Name = t.Name,
            Status = t.Status.ToString(),
            CreatedAt = t.CreatedAt,
            PlayerIds = t.PlayerIds
        }).ToList();
    }
}

