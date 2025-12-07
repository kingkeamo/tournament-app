using MediatR;
using TournamentApp.Application.Commands;
using TournamentApp.Application.Interfaces;
using TournamentApp.Domain.Entities;

namespace TournamentApp.Application.Handlers;

public class AddPlayerHandler : IRequestHandler<AddPlayerCommand, Guid>
{
    private readonly IPlayerRepository _repository;

    public AddPlayerHandler(IPlayerRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(AddPlayerCommand request, CancellationToken cancellationToken)
    {
        var player = new Player
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            CreatedAt = DateTime.UtcNow
        };

        return await _repository.CreateAsync(player);
    }
}

