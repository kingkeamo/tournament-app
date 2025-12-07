using MediatR;

namespace TournamentApp.Application.Commands;

public class GenerateBracketCommand : IRequest
{
    public Guid TournamentId { get; set; }
}

