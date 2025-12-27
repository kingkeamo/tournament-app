using System.Net;
using FluentAssertions;
using FluentValidation.Results;
using TournamentApp.Shared;
using TournamentApp.Web.Contracts.Services;
using TournamentApp.Web.Responses;
using TournamentApp.Web.Services;
using TournamentApp.WebTests.Helpers;

namespace TournamentApp.WebTests.Services;

public class WhenTestingPlayerService
{
    [Fact]
    public async Task GetPlayers_ShouldReturnPlayers_WhenApiReturnsSuccess()
    {
        // Arrange
        var players = new List<PlayerDto>
        {
            new PlayerDto { Id = Guid.NewGuid(), Name = "Player 1", CreatedAt = DateTime.UtcNow },
            new PlayerDto { Id = Guid.NewGuid(), Name = "Player 2", CreatedAt = DateTime.UtcNow }
        };

        var response = new DataResponse<IEnumerable<PlayerDto>>
        {
            Data = players,
            ValidationErrors = new List<ValidationFailure>(),
            ErrorMessage = string.Empty
        };

        var handler = new MockHttpMessageHandler(response, HttpStatusCode.OK);
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.test.com/") };
        var service = new PlayerService(httpClient);

        // Act
        var result = await service.GetPlayers();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.Data!.First().Name.Should().Be("Player 1");
    }

    [Fact]
    public async Task GetPlayers_ShouldReturnFailure_WhenApiReturnsValidationErrors()
    {
        // Arrange
        var validationErrors = new List<ValidationFailure>
        {
            new ValidationFailure("Error", "Failed to load players")
        };

        var response = new DataResponse<IEnumerable<PlayerDto>>
        {
            Data = null,
            ValidationErrors = validationErrors,
            ErrorMessage = string.Empty
        };

        var handler = new MockHttpMessageHandler(response, HttpStatusCode.BadRequest);
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.test.com/") };
        var service = new PlayerService(httpClient);

        // Act
        var result = await service.GetPlayers();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ValidationErrors.Should().HaveCount(1);
        result.ValidationErrors.First().ErrorMessage.Should().Be("Failed to load players");
    }

    [Fact]
    public async Task AddPlayer_ShouldReturnNewId_WhenApiReturnsSuccess()
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
        var service = new PlayerService(httpClient);
        var viewModel = new AddPlayerViewModel { Name = "New Player" };

        // Act
        var result = await service.AddPlayer(viewModel);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.NewId.Should().Be(newId);
    }

    [Fact]
    public async Task AddPlayer_ShouldReturnFailure_WhenApiReturnsValidationErrors()
    {
        // Arrange
        var validationErrors = new List<ValidationFailure>
        {
            new ValidationFailure("Name", "Player name is required")
        };

        var response = new CreateResponse
        {
            NewId = Guid.Empty,
            ValidationErrors = validationErrors,
            ErrorMessage = string.Empty
        };

        var handler = new MockHttpMessageHandler(response, HttpStatusCode.BadRequest);
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.test.com/") };
        var service = new PlayerService(httpClient);
        var viewModel = new AddPlayerViewModel { Name = string.Empty };

        // Act
        var result = await service.AddPlayer(viewModel);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ValidationErrors.Should().HaveCount(1);
        result.ValidationErrors.First().PropertyName.Should().Be("Name");
    }
}
