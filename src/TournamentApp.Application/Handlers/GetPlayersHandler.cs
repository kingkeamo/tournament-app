using MediatR;
using TournamentApp.Application.DTOs;
using TournamentApp.Application.Interfaces;
using TournamentApp.Application.Queries;

namespace TournamentApp.Application.Handlers;

public class GetPlayersHandler : IRequestHandler<GetPlayersQuery, List<PlayerDto>>
{
    private readonly IPlayerRepository _repository;

    public GetPlayersHandler(IPlayerRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<PlayerDto>> Handle(GetPlayersQuery request, CancellationToken cancellationToken)
    {
        var players = await _repository.GetAllAsync();
        return players.Select(p => new PlayerDto
        {
            Id = p.Id,
            Name = p.Name,
            CreatedAt = p.CreatedAt
        }).ToList();
    }
}

