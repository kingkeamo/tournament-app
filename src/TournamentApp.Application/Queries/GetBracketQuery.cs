using MediatR;
using TournamentApp.Shared;

namespace TournamentApp.Application.Queries;

public class GetBracketQuery : IRequest<BracketDto?>
{
    public Guid TournamentId { get; set; }
}

