using MediatR;

namespace TournamentApp.Application.Commands;

public class AddPlayerToTournamentCommand : IRequest
{
    public Guid TournamentId { get; set; }
    public Guid PlayerId { get; set; }
}

