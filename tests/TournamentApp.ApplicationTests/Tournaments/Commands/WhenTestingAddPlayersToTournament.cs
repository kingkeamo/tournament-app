using FluentAssertions;
using NSubstitute;
using TournamentApp.Application.Interfaces;
using TournamentApp.Application.Tournaments.Commands;
using TournamentApp.Domain.Entities;

namespace TournamentApp.ApplicationTests.Tournaments.Commands;

public class WhenTestingAddPlayersToTournament
{
    [Fact]
    public async Task ItShouldReturnSuccessWhenTournamentAndPlayerExist()
    {
        var tournamentRepository = Substitute.For<ITournamentRepository>();
        var playerRepository = Substitute.For<IPlayerRepository>();
        var handler = new AddPlayerToTournamentHandler(tournamentRepository, playerRepository);
        
        var tournamentId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var command = new AddPlayerToTournamentCommand 
        { 
            TournamentId = tournamentId, 
            PlayerIds = new List<Guid> { playerId }
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
        tournamentRepository.GetPlayerIdsAsync(tournamentId).Returns(new List<Guid>());
        playerRepository.GetByIdAsync(playerId).Returns(player);

        var response = await handler.Handle(command, CancellationToken.None);

        response.IsSuccess.Should().BeTrue();
        response.ErrorMessage.Should().BeEmpty();

        await tournamentRepository.Received(1).AddPlayersAsync(tournamentId, Arg.Is<List<Guid>>(ids => ids.Contains(playerId) && ids.Count == 1));
    }

    [Fact]
    public async Task ItShouldReturnFailureWhenTournamentNotFound()
    {
        var tournamentRepository = Substitute.For<ITournamentRepository>();
        var playerRepository = Substitute.For<IPlayerRepository>();
        var handler = new AddPlayerToTournamentHandler(tournamentRepository, playerRepository);
        
        var tournamentId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var command = new AddPlayerToTournamentCommand 
        { 
            TournamentId = tournamentId, 
            PlayerIds = new List<Guid> { playerId }
        };

        tournamentRepository.GetByIdAsync(tournamentId).Returns((Tournament?)null);

        var response = await handler.Handle(command, CancellationToken.None);

        response.IsFailure.Should().BeTrue();
        response.ErrorMessage.Should().Contain(tournamentId.ToString());

        await tournamentRepository.DidNotReceive().AddPlayersAsync(Arg.Any<Guid>(), Arg.Any<List<Guid>>());
    }

    [Fact]
    public async Task ItShouldReturnFailureWhenPlayerNotFound()
    {
        var tournamentRepository = Substitute.For<ITournamentRepository>();
        var playerRepository = Substitute.For<IPlayerRepository>();
        var handler = new AddPlayerToTournamentHandler(tournamentRepository, playerRepository);
        
        var tournamentId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var command = new AddPlayerToTournamentCommand 
        { 
            TournamentId = tournamentId, 
            PlayerIds = new List<Guid> { playerId }
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
        tournamentRepository.GetPlayerIdsAsync(tournamentId).Returns(new List<Guid>());
        playerRepository.GetByIdAsync(playerId).Returns((Player?)null);

        var response = await handler.Handle(command, CancellationToken.None);

        response.IsFailure.Should().BeTrue();
        response.ErrorMessage.Should().Contain("not found");

        await tournamentRepository.DidNotReceive().AddPlayersAsync(Arg.Any<Guid>(), Arg.Any<List<Guid>>());
    }

    [Fact]
    public async Task ItShouldReturnSuccessWhenAddingMultiplePlayers()
    {
        var tournamentRepository = Substitute.For<ITournamentRepository>();
        var playerRepository = Substitute.For<IPlayerRepository>();
        var handler = new AddPlayerToTournamentHandler(tournamentRepository, playerRepository);
        
        var tournamentId = Guid.NewGuid();
        var playerId1 = Guid.NewGuid();
        var playerId2 = Guid.NewGuid();
        var command = new AddPlayerToTournamentCommand 
        { 
            TournamentId = tournamentId, 
            PlayerIds = new List<Guid> { playerId1, playerId2 }
        };

        var tournament = new Tournament 
        { 
            Id = tournamentId, 
            Name = "Test Tournament",
            Status = TournamentStatus.Draft,
            CreatedAt = DateTime.UtcNow,
            PlayerIds = new List<Guid>()
        };
        var player1 = new Player { Id = playerId1, Name = "Player 1", CreatedAt = DateTime.UtcNow };
        var player2 = new Player { Id = playerId2, Name = "Player 2", CreatedAt = DateTime.UtcNow };

        tournamentRepository.GetByIdAsync(tournamentId).Returns(tournament);
        tournamentRepository.GetPlayerIdsAsync(tournamentId).Returns(new List<Guid>());
        playerRepository.GetByIdAsync(playerId1).Returns(player1);
        playerRepository.GetByIdAsync(playerId2).Returns(player2);

        var response = await handler.Handle(command, CancellationToken.None);

        response.IsSuccess.Should().BeTrue();
        await tournamentRepository.Received(1).AddPlayersAsync(tournamentId, Arg.Is<List<Guid>>(ids => ids.Contains(playerId1) && ids.Contains(playerId2) && ids.Count == 2));
    }

    [Fact]
    public async Task ItShouldReturnFailureWhenAllPlayersAreDuplicates()
    {
        var tournamentRepository = Substitute.For<ITournamentRepository>();
        var playerRepository = Substitute.For<IPlayerRepository>();
        var handler = new AddPlayerToTournamentHandler(tournamentRepository, playerRepository);
        
        var tournamentId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var command = new AddPlayerToTournamentCommand 
        { 
            TournamentId = tournamentId, 
            PlayerIds = new List<Guid> { playerId }
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
        tournamentRepository.GetPlayerIdsAsync(tournamentId).Returns(new List<Guid> { playerId });

        var response = await handler.Handle(command, CancellationToken.None);

        response.IsFailure.Should().BeTrue();
        response.ErrorMessage.Should().Contain("already in this tournament");
        await tournamentRepository.DidNotReceive().AddPlayersAsync(Arg.Any<Guid>(), Arg.Any<List<Guid>>());
    }

    [Fact]
    public async Task ItShouldReturnFailureWhenNoPlayersProvided()
    {
        var tournamentRepository = Substitute.For<ITournamentRepository>();
        var playerRepository = Substitute.For<IPlayerRepository>();
        var handler = new AddPlayerToTournamentHandler(tournamentRepository, playerRepository);
        
        var tournamentId = Guid.NewGuid();
        var command = new AddPlayerToTournamentCommand 
        { 
            TournamentId = tournamentId, 
            PlayerIds = new List<Guid>()
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

        var response = await handler.Handle(command, CancellationToken.None);

        response.IsFailure.Should().BeTrue();
        response.ErrorMessage.Should().Contain("At least one player must be selected");
        await tournamentRepository.DidNotReceive().AddPlayersAsync(Arg.Any<Guid>(), Arg.Any<List<Guid>>());
    }
}

