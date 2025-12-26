using FluentAssertions;
using NSubstitute;
using TournamentApp.Application.Interfaces;
using TournamentApp.Application.Tournaments.Queries;
using TournamentApp.Domain.Entities;

namespace TournamentApp.ApplicationTests.Tournaments.Queries;

public class WhenTestingGetTournament
{
    [Fact]
    public async Task ItShouldReturnTournamentWhenTournamentExists()
    {
        // Arrange
        var repository = Substitute.For<ITournamentRepository>();
        var handler = new GetTournamentQueryHandler(repository);
        var tournamentId = Guid.NewGuid();
        var query = new GetTournamentQuery { TournamentId = tournamentId };

        var tournament = new Tournament
        {
            Id = tournamentId,
            Name = "Test Tournament",
            Status = TournamentStatus.Draft,
            CreatedAt = DateTime.UtcNow,
            PlayerIds = new List<Guid>()
        };

        repository.GetByIdAsync(tournamentId).Returns(tournament);

        // Act
        var response = await handler.Handle(query, CancellationToken.None);

        // Assert
        response.IsSuccess.Should().BeTrue();
        response.Data.Should().NotBeNull();
        response.Data!.Id.Should().Be(tournamentId);
        response.Data.Name.Should().Be("Test Tournament");
        response.ErrorMessage.Should().BeEmpty();
    }

    [Fact]
    public async Task ItShouldReturnFailureWhenTournamentNotFound()
    {
        // Arrange
        var repository = Substitute.For<ITournamentRepository>();
        var handler = new GetTournamentQueryHandler(repository);
        var tournamentId = Guid.NewGuid();
        var query = new GetTournamentQuery { TournamentId = tournamentId };

        repository.GetByIdAsync(tournamentId).Returns((Tournament?)null);

        // Act
        var response = await handler.Handle(query, CancellationToken.None);

        // Assert
        response.IsFailure.Should().BeTrue();
        response.Data.Should().BeNull();
        response.ErrorMessage.Should().Contain(tournamentId.ToString());
    }
}

