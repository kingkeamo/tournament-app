using Bunit;
using FluentAssertions;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using NSubstitute;
using TournamentApp.Shared;
using TournamentApp.Web.Contracts.Services;
using TournamentApp.Web.Pages.Players;
using TournamentApp.Web.Responses;
using TournamentApp.WebTests.Helpers;

namespace TournamentApp.WebTests.Pages.AllPlayers;

public class WhenTestingAllPlayers : TestContext
{
    private readonly IPlayerService playerService;
    private readonly ISnackbar snackbar;
    private readonly IDialogService dialogService;

    public WhenTestingAllPlayers()
    {
        this.AddMudBlazorServices();
        
        playerService = Substitute.For<IPlayerService>();
        snackbar = Substitute.For<ISnackbar>();
        dialogService = Substitute.For<IDialogService>();

        Services.AddScoped(_ => playerService);
        Services.AddScoped(_ => snackbar);
        Services.AddScoped(_ => dialogService);
    }

    [Fact]
    public void ItShouldDisplayPlayersWhenPlayersAreLoaded()
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

        playerService.GetPlayers().Returns(Task.FromResult(response));

        // Act
        var component = RenderComponent<TournamentApp.Web.Pages.Players.AllPlayers>();

        // Assert
        component.FindAll("tr").Count.Should().BeGreaterThan(1);
        component.Markup.Should().Contain("Player 1");
        component.Markup.Should().Contain("Player 2");
    }

    [Fact]
    public void ItShouldDisplayEmptyStateWhenNoPlayersExist()
    {
        // Arrange
        var response = new DataResponse<IEnumerable<PlayerDto>>
        {
            Data = new List<PlayerDto>(),
            ValidationErrors = new List<ValidationFailure>(),
            ErrorMessage = string.Empty
        };

        playerService.GetPlayers().Returns(Task.FromResult(response));

        // Act
        var component = RenderComponent<TournamentApp.Web.Pages.Players.AllPlayers>();

        // Assert
        component.Markup.Should().Contain("No players");
    }

    [Fact]
    public void ItShouldDisplayErrorWhenApiReturnsValidationErrors()
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

        playerService.GetPlayers().Returns(Task.FromResult(response));

        // Act
        var component = RenderComponent<TournamentApp.Web.Pages.Players.AllPlayers>();

        // Assert
        snackbar.Received().Add("Failed to load players", Severity.Error);
    }

    [Fact]
    public void ItShouldDisplayErrorWhenApiReturnsErrorMessage()
    {
        // Arrange
        var response = new DataResponse<IEnumerable<PlayerDto>>
        {
            Data = null,
            ValidationErrors = new List<ValidationFailure>(),
            ErrorMessage = "Network error occurred"
        };

        playerService.GetPlayers().Returns(Task.FromResult(response));

        // Act
        var component = RenderComponent<TournamentApp.Web.Pages.Players.AllPlayers>();

        // Assert
        snackbar.Received().Add("Network error occurred", Severity.Error);
    }

    [Fact]
    public void ItShouldHaveCreateButtonThatOpensDialog()
    {
        // Arrange
        var response = new DataResponse<IEnumerable<PlayerDto>>
        {
            Data = new List<PlayerDto>(),
            ValidationErrors = new List<ValidationFailure>(),
            ErrorMessage = string.Empty
        };

        playerService.GetPlayers().Returns(Task.FromResult(response));

        // Act
        var component = RenderComponent<TournamentApp.Web.Pages.Players.AllPlayers>();
        var createButton = component.Find("button");

        // Assert
        createButton.Should().NotBeNull();
    }
}

