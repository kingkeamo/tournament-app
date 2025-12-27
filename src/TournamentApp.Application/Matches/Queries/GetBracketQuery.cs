using FluentValidation;
using MediatR;
using TournamentApp.Application.Common.Responses;
using TournamentApp.Application.Interfaces;
using TournamentApp.Shared;

namespace TournamentApp.Application.Matches.Queries;

public class GetBracketQuery : IRequest<GetBracketResponse>
{
    public Guid TournamentId { get; set; }
}

public class GetBracketQueryHandler : IRequestHandler<GetBracketQuery, GetBracketResponse>
{
    private readonly IMatchRepository _matchRepository;
    private readonly ITournamentRepository _tournamentRepository;

    public GetBracketQueryHandler(IMatchRepository matchRepository, ITournamentRepository tournamentRepository)
    {
        _matchRepository = matchRepository;
        _tournamentRepository = tournamentRepository;
    }

    public async Task<GetBracketResponse> Handle(GetBracketQuery request, CancellationToken cancellationToken)
    {
        // Business rule: Tournament must exist
        var tournament = await _tournamentRepository.GetByIdAsync(request.TournamentId);
        if (tournament == null)
        {
            return new GetBracketResponse
            {
                ErrorMessage = $"Tournament with ID {request.TournamentId} not found"
            };
        }

        var matches = await _matchRepository.GetByTournamentIdAsync(request.TournamentId);
        var matchDtos = matches.Select(m => new MatchDto
        {
            Id = m.Id,
            TournamentId = m.TournamentId,
            Round = m.Round,
            Position = m.Position,
            Player1Id = m.Player1Id,
            Player2Id = m.Player2Id,
            Score1 = m.Score1,
            Score2 = m.Score2,
            WinnerId = m.WinnerId,
            Status = m.Status.ToString(),
            CreatedAt = m.CreatedAt
        }).ToList();

        var matchesByRound = matchDtos.GroupBy(m => m.Round)
            .ToDictionary(g => g.Key, g => g.OrderBy(m => m.Position).ToList());

        var bracketDto = new BracketDto
        {
            TournamentId = request.TournamentId,
            Matches = matchDtos,
            MatchesByRound = matchesByRound
        };

        return new GetBracketResponse
        {
            Data = bracketDto
        };
    }
}

public class GetBracketResponse : ValidatedResponse
{
    public BracketDto? Data { get; set; }
}

public class GetBracketQueryValidator : AbstractValidator<GetBracketQuery>
{
    public GetBracketQueryValidator()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.TournamentId).NotEmpty().WithMessage("Tournament ID is required");
    }
}







