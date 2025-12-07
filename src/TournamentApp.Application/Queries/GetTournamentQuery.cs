using MediatR;
using TournamentApp.Application.DTOs;

namespace TournamentApp.Application.Queries;

public class GetTournamentQuery : IRequest<TournamentDto?>
{
    public Guid TournamentId { get; set; }
}

