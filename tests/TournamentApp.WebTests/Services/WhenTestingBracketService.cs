using System.Net;
using FluentAssertions;
using FluentValidation.Results;
using TournamentApp.Shared;
using TournamentApp.Web.Responses;
using TournamentApp.Web.Services;
using TournamentApp.WebTests.Helpers;

namespace TournamentApp.WebTests.Services;

public class WhenTestingBracketService
{
    [Fact]
    public async Task GenerateBracket_ShouldReturnSuccess_WhenApiReturnsSuccess()
    {
        // Arrange
        var response = new Response
        {
            ValidationErrors = new List<ValidationFailure>(),
            ErrorMessage = string.Empty
        };

        var handler = new MockHttpMessageHandler(response, HttpStatusCode.OK);
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.test.com/") };
        var service = new BracketService(httpClient);
        var tournamentId = Guid.NewGuid();

        // Act
        var result = await service.GenerateBracket(tournamentId);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task GenerateBracket_ShouldReturnFailure_WhenApiReturnsValidationErrors()
    {
        // Arrange
        var validationErrors = new List<ValidationFailure>
        {
            new ValidationFailure("Error", "At least 2 players are required to generate a bracket.")
        };

        var response = new Response
        {
            ValidationErrors = validationErrors,
            ErrorMessage = string.Empty
        };

        var handler = new MockHttpMessageHandler(response, HttpStatusCode.BadRequest);
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.test.com/") };
        var service = new BracketService(httpClient);
        var tournamentId = Guid.NewGuid();

        // Act
        var result = await service.GenerateBracket(tournamentId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ValidationErrors.Should().HaveCount(1);
        result.ValidationErrors.First().ErrorMessage.Should().Be("At least 2 players are required to generate a bracket.");
    }

    [Fact]
    public async Task GetBracket_ShouldReturnBracket_WhenApiReturnsSuccess()
    {
        // Arrange
        var tournamentId = Guid.NewGuid();
        var bracket = new BracketDto
        {
            TournamentId = tournamentId,
            Matches = new List<MatchDto>
            {
                new MatchDto
                {
                    Id = Guid.NewGuid(),
                    TournamentId = tournamentId,
                    Round = 1,
                    Position = 1,
                    Player1Id = Guid.NewGuid(),
                    Player2Id = Guid.NewGuid(),
                    Score1 = 0,
                    Score2 = 0,
                    Status = "Pending"
                }
            },
            MatchesByRound = new Dictionary<int, List<MatchDto>>
            {
                { 1, new List<MatchDto>() }
            }
        };

        var response = new DataResponse<BracketDto>
        {
            Data = bracket,
            ValidationErrors = new List<ValidationFailure>(),
            ErrorMessage = string.Empty
        };

        var handler = new MockHttpMessageHandler(response, HttpStatusCode.OK);
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.test.com/") };
        var service = new BracketService(httpClient);

        // Act
        var result = await service.GetBracket(tournamentId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.TournamentId.Should().Be(tournamentId);
        result.Data.Matches.Should().HaveCount(1);
        result.Data.MatchesByRound.Should().ContainKey(1);
    }

    [Fact]
    public async Task GetBracket_ShouldReturnFailure_WhenApiReturnsError()
    {
        // Arrange
        var response = new DataResponse<BracketDto>
        {
            Data = null!,
            ValidationErrors = new List<ValidationFailure>(),
            ErrorMessage = "Bracket not found"
        };

        var handler = new MockHttpMessageHandler(response, HttpStatusCode.NotFound);
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.test.com/") };
        var service = new BracketService(httpClient);
        var tournamentId = Guid.NewGuid();

        // Act
        var result = await service.GetBracket(tournamentId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Be("Bracket not found");
    }
}

