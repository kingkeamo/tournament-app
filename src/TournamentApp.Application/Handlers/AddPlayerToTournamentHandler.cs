using MediatR;
using TournamentApp.Application.Commands;
using TournamentApp.Application.Interfaces;

namespace TournamentApp.Application.Handlers;

public class AddPlayerToTournamentHandler : IRequestHandler<AddPlayerToTournamentCommand>
{
    private readonly ITournamentRepository _repository;

    public AddPlayerToTournamentHandler(ITournamentRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(AddPlayerToTournamentCommand request, CancellationToken cancellationToken)
    {
        await _repository.AddPlayerAsync(request.TournamentId, request.PlayerId);
    }
}

