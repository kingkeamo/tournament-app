using MediatR;

namespace TournamentApp.Application.Commands;

public class CreateTournamentCommand : IRequest<Guid>
{
    public string Name { get; set; } = string.Empty;
}

