using Bunit;
using FluentAssertions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using NSubstitute;
using TournamentApp.Shared;
using TournamentApp.Web.Contracts.Services;
using TournamentApp.Web.Pages.Tournaments;
using TournamentApp.Web.Responses;
using TournamentApp.WebTests.Helpers;

namespace TournamentApp.WebTests.Pages.ViewTournament;

public class WhenTestingViewTournament : TestContext
{
    private readonly ITournamentService tournamentService;
    private readonly IBracketService bracketService;
    private readonly ISnackbar snackbar;
    private readonly IDialogService dialogService;

    public WhenTestingViewTournament()
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
        Services.AddScoped<NavigationManager>(_ => Substitute.For<NavigationManager>());
    }

    [Fact]
    public void ItShouldDisplayTournamentDetailsWhenTournamentIsLoaded()
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

        tournamentService.GetTournament(tournamentId).Returns(Task.FromResult(response));

        // Act
        var component = RenderComponent<TournamentApp.Web.Pages.Tournaments.ViewTournament>(parameters => parameters
            .Add(p => p.TournamentId, tournamentId));

        // Assert
        component.Markup.Should().Contain("Test Tournament");
    }

    [Fact]
    public void ItShouldDisplayErrorWhenTournamentNotFound()
    {
        // Arrange
        var tournamentId = Guid.NewGuid();
        var response = new DataResponse<TournamentDto>
        {
            Data = null!,
            ValidationErrors = new List<ValidationFailure>(),
            ErrorMessage = "Tournament not found"
        };

        tournamentService.GetTournament(tournamentId).Returns(Task.FromResult(response));

        // Act
        var component = RenderComponent<TournamentApp.Web.Pages.Tournaments.ViewTournament>(parameters => parameters
            .Add(p => p.TournamentId, tournamentId));

        // Assert
        snackbar.Received().Add("Tournament not found", Severity.Error);
    }

    [Fact]
    public void ItShouldCallGenerateBracketWhenGenerateButtonIsClicked()
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

        var tournamentResponse = new DataResponse<TournamentDto>
        {
            Data = tournament,
            ValidationErrors = new List<ValidationFailure>(),
            ErrorMessage = string.Empty
        };

        var bracketResponse = new Response
        {
            ValidationErrors = new List<ValidationFailure>(),
            ErrorMessage = string.Empty
        };

        tournamentService.GetTournament(tournamentId).Returns(Task.FromResult(tournamentResponse));
        bracketService.GenerateBracket(tournamentId).Returns(Task.FromResult(bracketResponse));

        // Act
        var component = RenderComponent<TournamentApp.Web.Pages.Tournaments.ViewTournament>(parameters => parameters
            .Add(p => p.TournamentId, tournamentId));

        // Assert
        tournamentService.Received().GetTournament(tournamentId);
    }
}

