using FluentAssertions;
using NSubstitute;
using TournamentApp.Application.Interfaces;
using TournamentApp.Application.Matches.Commands;
using TournamentApp.Domain.Entities;
using TournamentApp.Domain.Services;

namespace TournamentApp.ApplicationTests.Matches.Commands;

public class WhenTestingUpdateMatchScore
{
    [Fact]
    public async Task ItShouldReturnSuccessWhenMatchExistsWithValidScores()
    {
        // Arrange
        var matchRepository = Substitute.For<IMatchRepository>();
        var tournamentRepository = Substitute.For<ITournamentRepository>();
        var bracketGenerator = new BracketGenerator();
        var handler = new UpdateMatchScoreHandler(matchRepository, tournamentRepository, bracketGenerator);
        
        var matchId = Guid.NewGuid();
        var tournamentId = Guid.NewGuid();
        var player1Id = Guid.NewGuid();
        var player2Id = Guid.NewGuid();
        
        var command = new UpdateMatchScoreCommand 
        { 
            MatchId = matchId,
            Score1 = 10,
            Score2 = 5
        };

        var match = new Match
        {
            Id = matchId,
            TournamentId = tournamentId,
            Round = 1,
            Position = 0,
            Player1Id = player1Id,
            Player2Id = player2Id,
            Status = MatchStatus.Pending
        };

        matchRepository.GetByIdAsync(matchId).Returns(match);
        matchRepository.GetByTournamentIdAsync(tournamentId).Returns(new List<Match> { match });

        // Act
        var response = await handler.Handle(command, CancellationToken.None);

        // Assert
        response.IsSuccess.Should().BeTrue();
        response.ErrorMessage.Should().BeEmpty();

        await matchRepository.Received(1).UpdateAsync(Arg.Is<Match>(m => 
            m.Id == matchId && 
            m.Score1 == 10 && 
            m.Score2 == 5 &&
            m.WinnerId == player1Id &&
            m.Status == MatchStatus.Completed));
    }

    [Fact]
    public async Task ItShouldReturnFailureWhenMatchNotFound()
    {
        // Arrange
        var matchRepository = Substitute.For<IMatchRepository>();
        var tournamentRepository = Substitute.For<ITournamentRepository>();
        var bracketGenerator = new BracketGenerator();
        var handler = new UpdateMatchScoreHandler(matchRepository, tournamentRepository, bracketGenerator);
        
        var matchId = Guid.NewGuid();
        var command = new UpdateMatchScoreCommand 
        { 
            MatchId = matchId,
            Score1 = 10,
            Score2 = 5
        };

        matchRepository.GetByIdAsync(matchId).Returns((Match?)null);

        // Act
        var response = await handler.Handle(command, CancellationToken.None);

        // Assert
        response.IsFailure.Should().BeTrue();
        response.ErrorMessage.Should().Contain(matchId.ToString());

        await matchRepository.DidNotReceive().UpdateAsync(Arg.Any<Match>());
    }

    [Fact]
    public async Task ItShouldReturnFailureWhenScoresAreEqual()
    {
        // Arrange
        var matchRepository = Substitute.For<IMatchRepository>();
        var tournamentRepository = Substitute.For<ITournamentRepository>();
        var bracketGenerator = new BracketGenerator();
        var handler = new UpdateMatchScoreHandler(matchRepository, tournamentRepository, bracketGenerator);
        
        var matchId = Guid.NewGuid();
        var match = new Match
        {
            Id = matchId,
            TournamentId = Guid.NewGuid(),
            Round = 1,
            Position = 0,
            Player1Id = Guid.NewGuid(),
            Player2Id = Guid.NewGuid(),
            Status = MatchStatus.Pending
        };
        
        var command = new UpdateMatchScoreCommand 
        { 
            MatchId = matchId,
            Score1 = 10,
            Score2 = 10 // Equal scores
        };

        matchRepository.GetByIdAsync(matchId).Returns(match);

        // Act
        var response = await handler.Handle(command, CancellationToken.None);

        // Assert
        response.IsFailure.Should().BeTrue();
        response.ErrorMessage.Should().Contain("cannot be equal");

        await matchRepository.DidNotReceive().UpdateAsync(Arg.Any<Match>());
    }
}

