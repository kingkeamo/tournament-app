using FluentValidation;
using MediatR;
using TournamentApp.Application.Common.Responses;
using TournamentApp.Application.Interfaces;
using TournamentApp.Domain.Entities;

namespace TournamentApp.Application.Players.Commands;

public class AddPlayerCommand : IRequest<AddPlayerResponse>
{
    public string Name { get; set; } = string.Empty;
}

public class AddPlayerHandler : IRequestHandler<AddPlayerCommand, AddPlayerResponse>
{
    private readonly IPlayerRepository _repository;

    public AddPlayerHandler(IPlayerRepository repository)
    {
        _repository = repository;
    }

    public async Task<AddPlayerResponse> Handle(AddPlayerCommand request, CancellationToken cancellationToken)
    {
        var player = new Player
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            CreatedAt = DateTime.UtcNow
        };

        var newId = await _repository.CreateAsync(player);
        
        return new AddPlayerResponse
        {
            NewId = newId
        };
    }
}

public class AddPlayerResponse : ValidatedResponse
{
    public Guid NewId { get; set; }
}

public class AddPlayerCommandValidator : AbstractValidator<AddPlayerCommand>
{
    public AddPlayerCommandValidator()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.Name).NotEmpty().WithMessage("Player name is required");
    }
}







