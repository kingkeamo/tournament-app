using MediatR;
using TournamentApp.Application.Commands;
using TournamentApp.Application.Interfaces;
using TournamentApp.Domain.Entities;

namespace TournamentApp.Application.Handlers;

public class CreateTournamentHandler : IRequestHandler<CreateTournamentCommand, Guid>
{
    private readonly ITournamentRepository _repository;

    public CreateTournamentHandler(ITournamentRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(CreateTournamentCommand request, CancellationToken cancellationToken)
    {
        var tournament = new Tournament
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Status = TournamentStatus.Draft,
            CreatedAt = DateTime.UtcNow
        };

        return await _repository.CreateAsync(tournament);
    }
}

