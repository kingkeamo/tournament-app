using System.Net;
using FluentAssertions;
using FluentValidation.Results;
using TournamentApp.Web.Contracts.Services;
using TournamentApp.Web.Responses;
using TournamentApp.Web.Services;
using TournamentApp.WebTests.Helpers;

namespace TournamentApp.WebTests.Services;

public class WhenTestingMatchService
{
    [Fact]
    public async Task UpdateMatchScore_ShouldReturnSuccess_WhenApiReturnsSuccess()
    {
        // Arrange
        var response = new Response
        {
            ValidationErrors = new List<ValidationFailure>(),
            ErrorMessage = string.Empty
        };

        var handler = new MockHttpMessageHandler(response, HttpStatusCode.OK);
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.test.com/") };
        var service = new MatchService(httpClient);
        var matchId = Guid.NewGuid();
        var viewModel = new UpdateMatchScoreViewModel
        {
            MatchId = matchId,
            Score1 = 5,
            Score2 = 3
        };

        // Act
        var result = await service.UpdateMatchScore(matchId, viewModel);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateMatchScore_ShouldReturnFailure_WhenApiReturnsValidationErrors()
    {
        // Arrange
        var validationErrors = new List<ValidationFailure>
        {
            new ValidationFailure("Score1", "Scores cannot be equal")
        };

        var response = new Response
        {
            ValidationErrors = validationErrors,
            ErrorMessage = string.Empty
        };

        var handler = new MockHttpMessageHandler(response, HttpStatusCode.BadRequest);
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.test.com/") };
        var service = new MatchService(httpClient);
        var matchId = Guid.NewGuid();
        var viewModel = new UpdateMatchScoreViewModel
        {
            MatchId = matchId,
            Score1 = 5,
            Score2 = 5
        };

        // Act
        var result = await service.UpdateMatchScore(matchId, viewModel);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ValidationErrors.Should().HaveCount(1);
        result.ValidationErrors.First().PropertyName.Should().Be("Score1");
    }
}

