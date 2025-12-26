using FluentAssertions;
using NSubstitute;
using TournamentApp.Application.Interfaces;
using TournamentApp.Application.Tournaments.Commands;
using TournamentApp.Domain.Entities;

namespace TournamentApp.ApplicationTests.Tournaments.Commands;

public class WhenTestingCreateTournament
{
    [Fact]
    public async Task ItShouldReturnNewIdWhenValidCommand()
    {
        // Arrange
        var repository = Substitute.For<ITournamentRepository>();
        var handler = new CreateTournamentHandler(repository);
        var command = new CreateTournamentCommand { Name = "Test Tournament" };
        var expectedId = Guid.NewGuid();

        repository.CreateAsync(Arg.Any<Tournament>())
            .Returns(expectedId);

        // Act
        var response = await handler.Handle(command, CancellationToken.None);

        // Assert
        response.IsSuccess.Should().BeTrue();
        response.NewId.Should().Be(expectedId);
        response.ErrorMessage.Should().BeEmpty();

        await repository.Received(1).CreateAsync(Arg.Is<Tournament>(t => 
            t.Name == command.Name && 
            t.Status == TournamentStatus.Draft &&
            t.Id != Guid.Empty));
    }
}

