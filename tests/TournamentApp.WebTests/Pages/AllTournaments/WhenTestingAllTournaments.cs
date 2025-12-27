using Bunit;
using FluentAssertions;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using NSubstitute;
using TournamentApp.Shared;
using TournamentApp.Web.Contracts.Services;
using TournamentApp.Web.Pages.Tournaments;
using TournamentApp.Web.Responses;
using TournamentApp.WebTests.Helpers;

namespace TournamentApp.WebTests.Pages.AllTournaments;

public class WhenTestingAllTournaments : TestContext
{
    private readonly ITournamentService tournamentService;
    private readonly IBracketService bracketService;
    private readonly ISnackbar snackbar;
    private readonly IDialogService dialogService;

    public WhenTestingAllTournaments()
    {
        this.AddMudBlazorServices();
        
        tournamentService = Substitute.For<ITournamentService>();
        bracketService = Substitute.For<IBracketService>();
        snackbar = Substitute.For<ISnackbar>();
        dialogService = Substitute.For<IDialogService>();

        Services.AddScoped(_ => tournamentService);
        Services.AddScoped(_ => bracketService);
        Services.AddScoped(_ => snackbar);
        Services.AddScoped(_ => dialogService);
    }

    [Fact]
    public void ItShouldDisplayTournamentsWhenTournamentsAreLoaded()
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

        tournamentService.GetTournaments().Returns(Task.FromResult(response));

        // Act
        var component = RenderComponent<TournamentApp.Web.Pages.Tournaments.AllTournaments>();

        // Assert
        component.Markup.Should().Contain("Tournament 1");
        component.Markup.Should().Contain("Tournament 2");
    }

    [Fact]
    public void ItShouldDisplayEmptyStateWhenNoTournamentsExist()
    {
        // Arrange
        var response = new DataResponse<IEnumerable<TournamentDto>>
        {
            Data = new List<TournamentDto>(),
            ValidationErrors = new List<ValidationFailure>(),
            ErrorMessage = string.Empty
        };

        tournamentService.GetTournaments().Returns(Task.FromResult(response));

        // Act
        var component = RenderComponent<TournamentApp.Web.Pages.Tournaments.AllTournaments>();

        // Assert
        component.Markup.Should().Contain("No tournaments");
    }

    [Fact]
    public void ItShouldDisplayErrorWhenApiReturnsValidationErrors()
    {
        // Arrange
        var validationErrors = new List<ValidationFailure>
        {
            new ValidationFailure("Error", "Failed to load tournaments")
        };

        var response = new DataResponse<IEnumerable<TournamentDto>>
        {
            Data = null,
            ValidationErrors = validationErrors,
            ErrorMessage = string.Empty
        };

        tournamentService.GetTournaments().Returns(Task.FromResult(response));

        // Act
        var component = RenderComponent<TournamentApp.Web.Pages.Tournaments.AllTournaments>();

        // Assert
        snackbar.Received().Add("Failed to load tournaments", Severity.Error);
    }

    [Fact]
    public void ItShouldCallGenerateBracketWhenGenerateButtonIsClicked()
    {
        // Arrange
        var tournamentId = Guid.NewGuid();
        var tournaments = new List<TournamentDto>
        {
            new TournamentDto { Id = tournamentId, Name = "Tournament 1", Status = "Draft", CreatedAt = DateTime.UtcNow }
        };

        var tournamentsResponse = new DataResponse<IEnumerable<TournamentDto>>
        {
            Data = tournaments,
            ValidationErrors = new List<ValidationFailure>(),
            ErrorMessage = string.Empty
        };

        var bracketResponse = new Response
        {
            ValidationErrors = new List<ValidationFailure>(),
            ErrorMessage = string.Empty
        };

        tournamentService.GetTournaments().Returns(Task.FromResult(tournamentsResponse));
        bracketService.GenerateBracket(tournamentId).Returns(Task.FromResult(bracketResponse));

        // Act
        var component = RenderComponent<TournamentApp.Web.Pages.Tournaments.AllTournaments>();

        // Assert
        tournamentService.Received().GetTournaments();
    }
}

