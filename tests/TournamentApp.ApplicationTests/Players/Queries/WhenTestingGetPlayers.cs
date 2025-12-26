using FluentAssertions;
using NSubstitute;
using TournamentApp.Application.Interfaces;
using TournamentApp.Application.Players.Queries;
using TournamentApp.Domain.Entities;

namespace TournamentApp.ApplicationTests.Players.Queries;

public class WhenTestingGetPlayers
{
    [Fact]
    public async Task ItShouldReturnPlayersWhenPlayersExist()
    {
        // Arrange
        var repository = Substitute.For<IPlayerRepository>();
        var handler = new GetPlayersQueryHandler(repository);
        var query = new GetPlayersQuery();

        var players = new List<Player>
        {
            new Player { Id = Guid.NewGuid(), Name = "Player 1", CreatedAt = DateTime.UtcNow },
            new Player { Id = Guid.NewGuid(), Name = "Player 2", CreatedAt = DateTime.UtcNow }
        };

        repository.GetAllAsync().Returns(players);

        // Act
        var response = await handler.Handle(query, CancellationToken.None);

        // Assert
        response.IsSuccess.Should().BeTrue();
        response.Data.Should().NotBeNull();
        response.Data.Should().HaveCount(2);
        response.Data!.First().Name.Should().Be("Player 1");
        response.ErrorMessage.Should().BeEmpty();
        response.ValidationErrors.Should().BeEmpty();
    }

    [Fact]
    public async Task ItShouldReturnEmptyListWhenNoPlayers()
    {
        // Arrange
        var repository = Substitute.For<IPlayerRepository>();
        var handler = new GetPlayersQueryHandler(repository);
        var query = new GetPlayersQuery();

        repository.GetAllAsync().Returns(new List<Player>());

        // Act
        var response = await handler.Handle(query, CancellationToken.None);

        // Assert
        response.IsSuccess.Should().BeTrue();
        response.Data.Should().NotBeNull();
        response.Data.Should().BeEmpty();
    }
}

