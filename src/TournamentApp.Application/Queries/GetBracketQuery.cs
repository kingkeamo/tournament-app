using MediatR;
using TournamentApp.Application.DTOs;

namespace TournamentApp.Application.Queries;

public class GetBracketQuery : IRequest<BracketDto?>
{
    public Guid TournamentId { get; set; }
}

