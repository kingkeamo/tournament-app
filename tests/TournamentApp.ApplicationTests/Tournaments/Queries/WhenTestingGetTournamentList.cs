using FluentAssertions;
using NSubstitute;
using TournamentApp.Application.Interfaces;
using TournamentApp.Application.Tournaments.Queries;
using TournamentApp.Domain.Entities;

namespace TournamentApp.ApplicationTests.Tournaments.Queries;

public class WhenTestingGetTournamentList
{
    [Fact]
    public async Task ItShouldReturnTournamentsWhenTournamentsExist()
    {
        // Arrange
        var repository = Substitute.For<ITournamentRepository>();
        var handler = new GetTournamentListQueryHandler(repository);
        var query = new GetTournamentListQuery();

        var tournaments = new List<Tournament>
        {
            new Tournament
            {
                Id = Guid.NewGuid(),
                Name = "Tournament 1",
                Status = TournamentStatus.Draft,
                CreatedAt = DateTime.UtcNow,
                PlayerIds = new List<Guid>()
            },
            new Tournament
            {
                Id = Guid.NewGuid(),
                Name = "Tournament 2",
                Status = TournamentStatus.Draft,
                CreatedAt = DateTime.UtcNow,
                PlayerIds = new List<Guid>()
            }
        };

        repository.GetAllAsync().Returns(tournaments);

        // Act
        var response = await handler.Handle(query, CancellationToken.None);

        // Assert
        response.IsSuccess.Should().BeTrue();
        response.Data.Should().NotBeNull();
        response.Data.Should().HaveCount(2);
        response.Data!.First().Name.Should().Be("Tournament 1");
        response.ErrorMessage.Should().BeEmpty();
    }

    [Fact]
    public async Task ItShouldReturnEmptyListWhenNoTournaments()
    {
        // Arrange
        var repository = Substitute.For<ITournamentRepository>();
        var handler = new GetTournamentListQueryHandler(repository);
        var query = new GetTournamentListQuery();

        repository.GetAllAsync().Returns(new List<Tournament>());

        // Act
        var response = await handler.Handle(query, CancellationToken.None);

        // Assert
        response.IsSuccess.Should().BeTrue();
        response.Data.Should().NotBeNull();
        response.Data.Should().BeEmpty();
    }
}

