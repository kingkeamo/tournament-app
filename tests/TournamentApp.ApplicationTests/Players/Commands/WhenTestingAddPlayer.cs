using FluentAssertions;
using NSubstitute;
using TournamentApp.Application.Interfaces;
using TournamentApp.Application.Players.Commands;
using TournamentApp.Domain.Entities;

namespace TournamentApp.ApplicationTests.Players.Commands;

public class WhenTestingAddPlayer
{
    [Fact]
    public async Task ItShouldReturnNewIdWhenValidCommand()
    {
        // Arrange
        var repository = Substitute.For<IPlayerRepository>();
        var handler = new AddPlayerHandler(repository);
        var command = new AddPlayerCommand { Name = "Test Player" };
        var expectedId = Guid.NewGuid();

        repository.CreateAsync(Arg.Any<Player>())
            .Returns(expectedId);

        // Act
        var response = await handler.Handle(command, CancellationToken.None);

        // Assert
        response.IsSuccess.Should().BeTrue();
        response.NewId.Should().Be(expectedId);
        response.ErrorMessage.Should().BeEmpty();
        response.ValidationErrors.Should().BeEmpty();

        await repository.Received(1).CreateAsync(Arg.Is<Player>(p => 
            p.Name == command.Name && 
            p.Id != Guid.Empty));
    }

    [Fact]
    public async Task ItShouldPropagateExceptionWhenRepositoryFails()
    {
        // Arrange
        var repository = Substitute.For<IPlayerRepository>();
        var handler = new AddPlayerHandler(repository);
        var command = new AddPlayerCommand { Name = "Test Player" };

        repository.CreateAsync(Arg.Any<Player>())
            .Returns(Task.FromException<Guid>(new InvalidOperationException("Database error")));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            handler.Handle(command, CancellationToken.None));
    }
}

