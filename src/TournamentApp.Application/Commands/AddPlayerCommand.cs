using MediatR;

namespace TournamentApp.Application.Commands;

public class AddPlayerCommand : IRequest<Guid>
{
    public string Name { get; set; } = string.Empty;
}

