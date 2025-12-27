using FluentAssertions;
using NSubstitute;
using TournamentApp.Application.Interfaces;
using TournamentApp.Application.Tournaments.Commands;
using TournamentApp.Domain.Entities;
using TournamentApp.Domain.Services;

namespace TournamentApp.ApplicationTests.Tournaments.Commands;

public class WhenTestingGenerateBracket
{
    [Fact]
    public async Task ItShouldReturnSuccessWhenTournamentExistsWithEnoughPlayers()
    {
        // Arrange
        var tournamentRepository = Substitute.For<ITournamentRepository>();
        var matchRepository = Substitute.For<IMatchRepository>();
        var bracketGenerator = new BracketGenerator();
        var handler = new GenerateBracketHandler(tournamentRepository, matchRepository, bracketGenerator);
        
        var tournamentId = Guid.NewGuid();
        var command = new GenerateBracketCommand { TournamentId = tournamentId };

        var tournament = new Tournament 
        { 
            Id = tournamentId, 
            Name = "Test Tournament",
            Status = TournamentStatus.Draft,
            CreatedAt = DateTime.UtcNow,
            PlayerIds = new List<Guid>()
        };
        var playerIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

        tournamentRepository.GetByIdAsync(tournamentId).Returns(tournament);
        tournamentRepository.GetPlayerIdsAsync(tournamentId).Returns(playerIds);

        // Act
        var response = await handler.Handle(command, CancellationToken.None);

        // Assert
        response.IsSuccess.Should().BeTrue();
        response.ErrorMessage.Should().BeEmpty();

        await matchRepository.Received(1).CreateManyAsync(Arg.Any<List<Match>>());
    }

    [Fact]
    public async Task ItShouldReturnFailureWhenTournamentNotFound()
    {
        // Arrange
        var tournamentRepository = Substitute.For<ITournamentRepository>();
        var matchRepository = Substitute.For<IMatchRepository>();
        var bracketGenerator = new BracketGenerator();
        var handler = new GenerateBracketHandler(tournamentRepository, matchRepository, bracketGenerator);
        
        var tournamentId = Guid.NewGuid();
        var command = new GenerateBracketCommand { TournamentId = tournamentId };

        tournamentRepository.GetByIdAsync(tournamentId).Returns((Tournament?)null);

        // Act
        var response = await handler.Handle(command, CancellationToken.None);

        // Assert
        response.IsFailure.Should().BeTrue();
        response.ErrorMessage.Should().Contain(tournamentId.ToString());

        await matchRepository.DidNotReceive().CreateManyAsync(Arg.Any<List<Match>>());
    }

    [Fact]
    public async Task ItShouldReturnFailureWhenNotEnoughPlayers()
    {
        // Arrange
        var tournamentRepository = Substitute.For<ITournamentRepository>();
        var matchRepository = Substitute.For<IMatchRepository>();
        var bracketGenerator = new BracketGenerator();
        var handler = new GenerateBracketHandler(tournamentRepository, matchRepository, bracketGenerator);
        
        var tournamentId = Guid.NewGuid();
        var command = new GenerateBracketCommand { TournamentId = tournamentId };

        var tournament = new Tournament 
        { 
            Id = tournamentId, 
            Name = "Test Tournament",
            Status = TournamentStatus.Draft,
            CreatedAt = DateTime.UtcNow,
            PlayerIds = new List<Guid>()
        };
        var playerIds = new List<Guid> { Guid.NewGuid() }; // Only 1 player

        tournamentRepository.GetByIdAsync(tournamentId).Returns(tournament);
        tournamentRepository.GetPlayerIdsAsync(tournamentId).Returns(playerIds);

        // Act
        var response = await handler.Handle(command, CancellationToken.None);

        // Assert
        response.IsFailure.Should().BeTrue();
        response.ErrorMessage.Should().Contain("At least 2 players");

        await matchRepository.DidNotReceive().CreateManyAsync(Arg.Any<List<Match>>());
    }
}

