using FluentAssertions;
using NSubstitute;
using TournamentApp.Application.Interfaces;
using TournamentApp.Application.Tournaments.Commands;
using TournamentApp.Domain.Entities;

namespace TournamentApp.ApplicationTests.Tournaments.Commands;

public class WhenTestingRemovePlayerFromTournament
{
    [Fact]
    public async Task ItShouldReturnSuccessWhenTournamentAndPlayerExist()
    {
        var tournamentRepository = Substitute.For<ITournamentRepository>();
        var playerRepository = Substitute.For<IPlayerRepository>();
        var handler = new RemovePlayerFromTournamentHandler(tournamentRepository, playerRepository);
        
        var tournamentId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var command = new RemovePlayerFromTournamentCommand 
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
            PlayerIds = new List<Guid> { playerId }
        };
        var player = new Player 
        { 
            Id = playerId, 
            Name = "Test Player",
            CreatedAt = DateTime.UtcNow
        };

        tournamentRepository.GetByIdAsync(tournamentId).Returns(tournament);
        tournamentRepository.GetPlayerIdsAsync(tournamentId).Returns(new List<Guid> { playerId });
        playerRepository.GetByIdAsync(playerId).Returns(player);

        var response = await handler.Handle(command, CancellationToken.None);

        response.IsSuccess.Should().BeTrue();
        response.ErrorMessage.Should().BeEmpty();

        await tournamentRepository.Received(1).RemovePlayerAsync(tournamentId, playerId);
    }

    [Fact]
    public async Task ItShouldReturnFailureWhenTournamentNotFound()
    {
        var tournamentRepository = Substitute.For<ITournamentRepository>();
        var playerRepository = Substitute.For<IPlayerRepository>();
        var handler = new RemovePlayerFromTournamentHandler(tournamentRepository, playerRepository);
        
        var tournamentId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var command = new RemovePlayerFromTournamentCommand 
        { 
            TournamentId = tournamentId, 
            PlayerId = playerId
        };

        tournamentRepository.GetByIdAsync(tournamentId).Returns((Tournament?)null);

        var response = await handler.Handle(command, CancellationToken.None);

        response.IsFailure.Should().BeTrue();
        response.ErrorMessage.Should().Contain(tournamentId.ToString());

        await tournamentRepository.DidNotReceive().RemovePlayerAsync(Arg.Any<Guid>(), Arg.Any<Guid>());
    }

    [Fact]
    public async Task ItShouldReturnFailureWhenPlayerNotFound()
    {
        var tournamentRepository = Substitute.For<ITournamentRepository>();
        var playerRepository = Substitute.For<IPlayerRepository>();
        var handler = new RemovePlayerFromTournamentHandler(tournamentRepository, playerRepository);
        
        var tournamentId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var command = new RemovePlayerFromTournamentCommand 
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

        var response = await handler.Handle(command, CancellationToken.None);

        response.IsFailure.Should().BeTrue();
        response.ErrorMessage.Should().Contain(playerId.ToString());

        await tournamentRepository.DidNotReceive().RemovePlayerAsync(Arg.Any<Guid>(), Arg.Any<Guid>());
    }

    [Fact]
    public async Task ItShouldReturnFailureWhenPlayerNotInTournament()
    {
        var tournamentRepository = Substitute.For<ITournamentRepository>();
        var playerRepository = Substitute.For<IPlayerRepository>();
        var handler = new RemovePlayerFromTournamentHandler(tournamentRepository, playerRepository);
        
        var tournamentId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var otherPlayerId = Guid.NewGuid();
        var command = new RemovePlayerFromTournamentCommand 
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
            PlayerIds = new List<Guid> { otherPlayerId }
        };
        var player = new Player 
        { 
            Id = playerId, 
            Name = "Test Player",
            CreatedAt = DateTime.UtcNow
        };

        tournamentRepository.GetByIdAsync(tournamentId).Returns(tournament);
        tournamentRepository.GetPlayerIdsAsync(tournamentId).Returns(new List<Guid> { otherPlayerId });
        playerRepository.GetByIdAsync(playerId).Returns(player);

        var response = await handler.Handle(command, CancellationToken.None);

        response.IsFailure.Should().BeTrue();
        response.ErrorMessage.Should().Contain("not in this tournament");

        await tournamentRepository.DidNotReceive().RemovePlayerAsync(Arg.Any<Guid>(), Arg.Any<Guid>());
    }
}

