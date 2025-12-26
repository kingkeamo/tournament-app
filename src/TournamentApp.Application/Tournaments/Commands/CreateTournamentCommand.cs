using FluentValidation;
using MediatR;
using TournamentApp.Application.Common.Responses;
using TournamentApp.Application.Interfaces;
using TournamentApp.Domain.Entities;

namespace TournamentApp.Application.Tournaments.Commands;

public class CreateTournamentCommand : IRequest<CreateTournamentResponse>
{
    public string Name { get; set; } = string.Empty;
}

public class CreateTournamentHandler : IRequestHandler<CreateTournamentCommand, CreateTournamentResponse>
{
    private readonly ITournamentRepository _repository;

    public CreateTournamentHandler(ITournamentRepository repository)
    {
        _repository = repository;
    }

    public async Task<CreateTournamentResponse> Handle(CreateTournamentCommand request, CancellationToken cancellationToken)
    {
        var tournament = new Tournament
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Status = TournamentStatus.Draft,
            CreatedAt = DateTime.UtcNow
        };

        var newId = await _repository.CreateAsync(tournament);

        return new CreateTournamentResponse
        {
            NewId = newId
        };
    }
}

public class CreateTournamentResponse : ValidatedResponse
{
    public Guid NewId { get; set; }
}

public class CreateTournamentCommandValidator : AbstractValidator<CreateTournamentCommand>
{
    public CreateTournamentCommandValidator()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.Name).NotEmpty().WithMessage("Tournament name is required");
    }
}



