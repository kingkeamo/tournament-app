using MediatR;
using TournamentApp.Application.Common.Responses;
using TournamentApp.Application.Interfaces;
using TournamentApp.Shared;

namespace TournamentApp.Application.Players.Queries;

public class GetPlayersQuery : IRequest<GetPlayersResponse>
{
}

public class GetPlayersQueryHandler : IRequestHandler<GetPlayersQuery, GetPlayersResponse>
{
    private readonly IPlayerRepository _repository;

    public GetPlayersQueryHandler(IPlayerRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetPlayersResponse> Handle(GetPlayersQuery request, CancellationToken cancellationToken)
    {
        var players = await _repository.GetAllAsync();
        var playerDtos = players.Select(p => new PlayerDto
        {
            Id = p.Id,
            Name = p.Name,
            CreatedAt = p.CreatedAt
        }).ToList();

        return new GetPlayersResponse
        {
            Data = playerDtos
        };
    }
}

public class GetPlayersResponse : ValidatedResponse
{
    public IEnumerable<PlayerDto>? Data { get; set; }
}







