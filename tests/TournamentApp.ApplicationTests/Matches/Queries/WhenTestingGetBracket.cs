using FluentAssertions;
using NSubstitute;
using TournamentApp.Application.Interfaces;
using TournamentApp.Application.Matches.Queries;
using TournamentApp.Domain.Entities;

namespace TournamentApp.ApplicationTests.Matches.Queries;

public class WhenTestingGetBracket
{
    [Fact]
    public async Task ItShouldReturnBracketWhenTournamentExists()
    {
        // Arrange
        var matchRepository = Substitute.For<IMatchRepository>();
        var tournamentRepository = Substitute.For<ITournamentRepository>();
        var handler = new GetBracketQueryHandler(matchRepository, tournamentRepository);
        
        var tournamentId = Guid.NewGuid();
        var query = new GetBracketQuery { TournamentId = tournamentId };

        var tournament = new Tournament
        {
            Id = tournamentId,
            Name = "Test Tournament",
            Status = TournamentStatus.Draft,
            CreatedAt = DateTime.UtcNow,
            PlayerIds = new List<Guid>()
        };

        var matches = new List<Match>
        {
            new Match
            {
                Id = Guid.NewGuid(),
                TournamentId = tournamentId,
                Round = 1,
                Position = 0,
                Player1Id = Guid.NewGuid(),
                Player2Id = Guid.NewGuid(),
                Status = MatchStatus.Pending
            }
        };

        tournamentRepository.GetByIdAsync(tournamentId).Returns(tournament);
        matchRepository.GetByTournamentIdAsync(tournamentId).Returns(matches);

        // Act
        var response = await handler.Handle(query, CancellationToken.None);

        // Assert
        response.IsSuccess.Should().BeTrue();
        response.Data.Should().NotBeNull();
        response.Data!.TournamentId.Should().Be(tournamentId);
        response.Data.Matches.Should().HaveCount(1);
        response.ErrorMessage.Should().BeEmpty();
    }

    [Fact]
    public async Task ItShouldReturnFailureWhenTournamentNotFound()
    {
        // Arrange
        var matchRepository = Substitute.For<IMatchRepository>();
        var tournamentRepository = Substitute.For<ITournamentRepository>();
        var handler = new GetBracketQueryHandler(matchRepository, tournamentRepository);
        
        var tournamentId = Guid.NewGuid();
        var query = new GetBracketQuery { TournamentId = tournamentId };

        tournamentRepository.GetByIdAsync(tournamentId).Returns((Tournament?)null);

        // Act
        var response = await handler.Handle(query, CancellationToken.None);

        // Assert
        response.IsFailure.Should().BeTrue();
        response.Data.Should().BeNull();
        response.ErrorMessage.Should().Contain(tournamentId.ToString());
    }
}

