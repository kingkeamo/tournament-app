using FluentAssertions;
using NSubstitute;
using TournamentApp.Application.Interfaces;
using TournamentApp.Application.Tournaments.Commands;
using TournamentApp.Domain.Entities;

namespace TournamentApp.ApplicationTests.Tournaments.Commands;

public class WhenTestingAddPlayerToTournament
{
    [Fact]
    public async Task ItShouldReturnSuccessWhenTournamentAndPlayerExist()
    {
        // Arrange
        var tournamentRepository = Substitute.For<ITournamentRepository>();
        var playerRepository = Substitute.For<IPlayerRepository>();
        var handler = new AddPlayerToTournamentHandler(tournamentRepository, playerRepository);
        
        var tournamentId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var command = new AddPlayerToTournamentCommand 
        { 
            TournamentId = tournamentId, 
            PlayerId = playerId 
        };

        var tournament = new Tournament 
        { 
            Id = tournamentId, 
            Name = "Test Tournament",
            Status = TournamentStatus.Draft,
            CreatedAt = DateTime.UtcNow,
            PlayerIds = new List<Guid>()
        };
        var player = new Player 
        { 
            Id = playerId, 
            Name = "Test Player",
            CreatedAt = DateTime.UtcNow
        };

        tournamentRepository.GetByIdAsync(tournamentId).Returns(tournament);
        playerRepository.GetByIdAsync(playerId).Returns(player);

        // Act
        var response = await handler.Handle(command, CancellationToken.None);

        // Assert
        response.IsSuccess.Should().BeTrue();
        response.ErrorMessage.Should().BeEmpty();

        await tournamentRepository.Received(1).AddPlayerAsync(tournamentId, playerId);
    }

    [Fact]
    public async Task ItShouldReturnFailureWhenTournamentNotFound()
    {
        // Arrange
        var tournamentRepository = Substitute.For<ITournamentRepository>();
        var playerRepository = Substitute.For<IPlayerRepository>();
        var handler = new AddPlayerToTournamentHandler(tournamentRepository, playerRepository);
        
        var tournamentId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var command = new AddPlayerToTournamentCommand 
        { 
            TournamentId = tournamentId, 
            PlayerId = playerId 
        };

        tournamentRepository.GetByIdAsync(tournamentId).Returns((Tournament?)null);

        // Act
        var response = await handler.Handle(command, CancellationToken.None);

        // Assert
        response.IsFailure.Should().BeTrue();
        response.ErrorMessage.Should().Contain(tournamentId.ToString());

        await tournamentRepository.DidNotReceive().AddPlayerAsync(Arg.Any<Guid>(), Arg.Any<Guid>());
    }

    [Fact]
    public async Task ItShouldReturnFailureWhenPlayerNotFound()
    {
        // Arrange
        var tournamentRepository = Substitute.For<ITournamentRepository>();
        var playerRepository = Substitute.For<IPlayerRepository>();
        var handler = new AddPlayerToTournamentHandler(tournamentRepository, playerRepository);
        
        var tournamentId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var command = new AddPlayerToTournamentCommand 
        { 
            TournamentId = tournamentId, 
            PlayerId = playerId 
        };

        var tournament = new Tournament 
        { 
            Id = tournamentId, 
            Name = "Test Tournament",
            Status = TournamentStatus.Draft,
            CreatedAt = DateTime.UtcNow,
            PlayerIds = new List<Guid>()
        };

        tournamentRepository.GetByIdAsync(tournamentId).Returns(tournament);
        playerRepository.GetByIdAsync(playerId).Returns((Player?)null);

        // Act
        var response = await handler.Handle(command, CancellationToken.None);

        // Assert
        response.IsFailure.Should().BeTrue();
        response.ErrorMessage.Should().Contain(playerId.ToString());

        await tournamentRepository.DidNotReceive().AddPlayerAsync(Arg.Any<Guid>(), Arg.Any<Guid>());
    }
}

