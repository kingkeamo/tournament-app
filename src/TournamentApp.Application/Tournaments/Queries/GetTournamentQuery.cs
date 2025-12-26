using FluentValidation;
using MediatR;
using TournamentApp.Application.Common.Responses;
using TournamentApp.Application.Interfaces;
using TournamentApp.Shared;

namespace TournamentApp.Application.Tournaments.Queries;

public class GetTournamentQuery : IRequest<GetTournamentResponse>
{
    public Guid TournamentId { get; set; }
}

public class GetTournamentQueryHandler : IRequestHandler<GetTournamentQuery, GetTournamentResponse>
{
    private readonly ITournamentRepository _repository;

    public GetTournamentQueryHandler(ITournamentRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetTournamentResponse> Handle(GetTournamentQuery request, CancellationToken cancellationToken)
    {
        var tournament = await _repository.GetByIdAsync(request.TournamentId);
        if (tournament == null)
        {
            return new GetTournamentResponse
            {
                ErrorMessage = $"Tournament with ID {request.TournamentId} not found"
            };
        }

        var tournamentDto = new TournamentDto
        {
            Id = tournament.Id,
            Name = tournament.Name,
            Status = tournament.Status.ToString(),
            CreatedAt = tournament.CreatedAt,
            PlayerIds = tournament.PlayerIds
        };

        return new GetTournamentResponse
        {
            Data = tournamentDto
        };
    }
}

public class GetTournamentResponse : ValidatedResponse
{
    public TournamentDto? Data { get; set; }
}

public class GetTournamentQueryValidator : AbstractValidator<GetTournamentQuery>
{
    public GetTournamentQueryValidator()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.TournamentId).NotEmpty().WithMessage("Tournament ID is required");
    }
}







