using MediatR;
using TournamentApp.Application.Common.Responses;
using TournamentApp.Application.Interfaces;
using TournamentApp.Shared;

namespace TournamentApp.Application.Tournaments.Queries;

public class GetTournamentListQuery : IRequest<GetTournamentListResponse>
{
}

public class GetTournamentListQueryHandler : IRequestHandler<GetTournamentListQuery, GetTournamentListResponse>
{
    private readonly ITournamentRepository _repository;

    public GetTournamentListQueryHandler(ITournamentRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetTournamentListResponse> Handle(GetTournamentListQuery request, CancellationToken cancellationToken)
    {
        var tournaments = await _repository.GetAllAsync();
        var tournamentDtos = tournaments.Select(t => new TournamentDto
        {
            Id = t.Id,
            Name = t.Name,
            Status = t.Status.ToString(),
            CreatedAt = t.CreatedAt,
            PlayerIds = t.PlayerIds
        }).ToList();

        return new GetTournamentListResponse
        {
            Data = tournamentDtos
        };
    }
}

public class GetTournamentListResponse : ValidatedResponse
{
    public IEnumerable<TournamentDto>? Data { get; set; }
}







