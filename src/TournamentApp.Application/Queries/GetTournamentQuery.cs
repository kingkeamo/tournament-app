using MediatR;
using TournamentApp.Shared;

namespace TournamentApp.Application.Queries;

public class GetTournamentQuery : IRequest<TournamentDto?>
{
    public Guid TournamentId { get; set; }
}

