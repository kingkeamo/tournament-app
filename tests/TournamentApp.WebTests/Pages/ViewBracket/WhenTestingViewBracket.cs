using Bunit;
using FluentAssertions;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using NSubstitute;
using TournamentApp.Shared;
using TournamentApp.Web.Contracts.Services;
using TournamentApp.Web.Pages.Bracket;
using TournamentApp.Web.Responses;
using TournamentApp.WebTests.Helpers;

namespace TournamentApp.WebTests.Pages.ViewBracket;

public class WhenTestingViewBracket : TestContext
{
    private readonly IBracketService bracketService;
    private readonly ITournamentService tournamentService;
    private readonly ISnackbar snackbar;

    public WhenTestingViewBracket()
    {
        this.AddMudBlazorServices();
        
        bracketService = Substitute.For<IBracketService>();
        tournamentService = Substitute.For<ITournamentService>();
        snackbar = Substitute.For<ISnackbar>();

        Services.AddScoped(_ => bracketService);
        Services.AddScoped(_ => tournamentService);
        Services.AddScoped(_ => snackbar);
    }

    [Fact]
    public void ItShouldDisplayBracketWhenBracketIsLoaded()
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

        var bracketResponse = new DataResponse<BracketDto>
        {
            Data = bracket,
            ValidationErrors = new List<ValidationFailure>(),
            ErrorMessage = string.Empty
        };

        var tournament = new TournamentDto
        {
            Id = tournamentId,
            Name = "Test Tournament",
            Status = "InProgress",
            CreatedAt = DateTime.UtcNow
        };

        var tournamentResponse = new DataResponse<TournamentDto>
        {
            Data = tournament,
            ValidationErrors = new List<ValidationFailure>(),
            ErrorMessage = string.Empty
        };

        bracketService.GetBracket(tournamentId).Returns(Task.FromResult(bracketResponse));
        tournamentService.GetTournament(tournamentId).Returns(Task.FromResult(tournamentResponse));

        // Act
        var component = RenderComponent<TournamentApp.Web.Pages.Bracket.ViewBracket>(parameters => parameters
            .Add(p => p.TournamentId, tournamentId));

        // Assert
        component.Markup.Should().Contain("Test Tournament");
        bracketService.Received().GetBracket(tournamentId);
        tournamentService.Received().GetTournament(tournamentId);
    }

    [Fact]
    public void ItShouldDisplayErrorWhenBracketNotFound()
    {
        // Arrange
        var tournamentId = Guid.NewGuid();
        var response = new DataResponse<BracketDto>
        {
            Data = null!,
            ValidationErrors = new List<ValidationFailure>(),
            ErrorMessage = "Bracket not found"
        };

        bracketService.GetBracket(tournamentId).Returns(Task.FromResult(response));

        // Act
        var component = RenderComponent<TournamentApp.Web.Pages.Bracket.ViewBracket>(parameters => parameters
            .Add(p => p.TournamentId, tournamentId));

        // Assert
        snackbar.Received().Add("Bracket not found", Severity.Error);
    }
}

