using System.Net;
using FluentAssertions;
using FluentValidation.Results;
using TournamentApp.Shared;
using TournamentApp.Web.Contracts.Services;
using TournamentApp.Web.Responses;
using TournamentApp.Web.Services;
using TournamentApp.WebTests.Helpers;

namespace TournamentApp.WebTests.Services;

public class WhenTestingTournamentService
{
    [Fact]
    public async Task GetTournaments_ShouldReturnTournaments_WhenApiReturnsSuccess()
    {
        // Arrange
        var tournaments = new List<TournamentDto>
        {
            new TournamentDto { Id = Guid.NewGuid(), Name = "Tournament 1", Status = "Draft", CreatedAt = DateTime.UtcNow },
            new TournamentDto { Id = Guid.NewGuid(), Name = "Tournament 2", Status = "InProgress", CreatedAt = DateTime.UtcNow }
        };

        var response = new DataResponse<IEnumerable<TournamentDto>>
        {
            Data = tournaments,
            ValidationErrors = new List<ValidationFailure>(),
            ErrorMessage = string.Empty
        };

        var handler = new MockHttpMessageHandler(response, HttpStatusCode.OK);
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.test.com/") };
        var service = new TournamentService(httpClient);

        // Act
        var result = await service.GetTournaments();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.Data!.First().Name.Should().Be("Tournament 1");
    }

    [Fact]
    public async Task GetTournament_ShouldReturnTournament_WhenApiReturnsSuccess()
    {
        // Arrange
        var tournamentId = Guid.NewGuid();
        var tournament = new TournamentDto
        {
            Id = tournamentId,
            Name = "Test Tournament",
            Status = "Draft",
            CreatedAt = DateTime.UtcNow
        };

        var response = new DataResponse<TournamentDto>
        {
            Data = tournament,
            ValidationErrors = new List<ValidationFailure>(),
            ErrorMessage = string.Empty
        };

        var handler = new MockHttpMessageHandler(response, HttpStatusCode.OK);
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.test.com/") };
        var service = new TournamentService(httpClient);

        // Act
        var result = await service.GetTournament(tournamentId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(tournamentId);
        result.Data.Name.Should().Be("Test Tournament");
    }

    [Fact]
    public async Task CreateTournament_ShouldReturnNewId_WhenApiReturnsSuccess()
    {
        // Arrange
        var newId = Guid.NewGuid();
        var response = new CreateResponse
        {
            NewId = newId,
            ValidationErrors = new List<ValidationFailure>(),
            ErrorMessage = string.Empty
        };

        var handler = new MockHttpMessageHandler(response, HttpStatusCode.OK);
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.test.com/") };
        var service = new TournamentService(httpClient);
        var viewModel = new CreateTournamentViewModel { Name = "New Tournament" };

        // Act
        var result = await service.CreateTournament(viewModel);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.NewId.Should().Be(newId);
    }

    [Fact]
    public async Task AddPlayerToTournament_ShouldReturnSuccess_WhenApiReturnsSuccess()
    {
        // Arrange
        var response = new Response
        {
            ValidationErrors = new List<ValidationFailure>(),
            ErrorMessage = string.Empty
        };

        var handler = new MockHttpMessageHandler(response, HttpStatusCode.OK);
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.test.com/") };
        var service = new TournamentService(httpClient);
        var viewModel = new AddPlayerToTournamentViewModel
        {
            TournamentId = Guid.NewGuid(),
            PlayerId = Guid.NewGuid()
        };

        // Act
        var result = await service.AddPlayerToTournament(viewModel.TournamentId, viewModel);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task AddPlayerToTournament_ShouldReturnFailure_WhenApiReturnsValidationErrors()
    {
        // Arrange
        var validationErrors = new List<ValidationFailure>
        {
            new ValidationFailure("PlayerId", "Player is already in tournament")
        };

        var response = new Response
        {
            ValidationErrors = validationErrors,
            ErrorMessage = string.Empty
        };

        var handler = new MockHttpMessageHandler(response, HttpStatusCode.BadRequest);
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.test.com/") };
        var service = new TournamentService(httpClient);
        var viewModel = new AddPlayerToTournamentViewModel
        {
            TournamentId = Guid.NewGuid(),
            PlayerId = Guid.NewGuid()
        };

        // Act
        var result = await service.AddPlayerToTournament(viewModel.TournamentId, viewModel);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ValidationErrors.Should().HaveCount(1);
    }
}

