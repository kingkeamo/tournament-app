using MediatR;
using TournamentApp.Application.Interfaces;
using TournamentApp.Application.Queries;
using TournamentApp.Shared;

namespace TournamentApp.Application.Handlers;

public class GetBracketHandler : IRequestHandler<GetBracketQuery, BracketDto?>
{
    private readonly IMatchRepository _matchRepository;
    private readonly ITournamentRepository _tournamentRepository;

    public GetBracketHandler(IMatchRepository matchRepository, ITournamentRepository tournamentRepository)
    {
        _matchRepository = matchRepository;
        _tournamentRepository = tournamentRepository;
    }

    public async Task<BracketDto?> Handle(GetBracketQuery request, CancellationToken cancellationToken)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(request.TournamentId);
        if (tournament == null)
        {
            return null;
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

        return new BracketDto
        {
            TournamentId = request.TournamentId,
            Matches = matchDtos,
            MatchesByRound = matchesByRound
        };
    }
}

