using Bunit;
using FluentAssertions;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using NSubstitute;
using TournamentApp.Web.Contracts.Services;
using TournamentApp.Web.Dialogs;
using TournamentApp.Web.Responses;
using TournamentApp.WebTests.Helpers;

namespace TournamentApp.WebTests.Dialogs.UpdateMatchScoreTests;

public class WhenTestingUpdateMatchScore : TestContext
{
    private readonly IMatchService _mockMatchService;
    private readonly ISnackbar _mockSnackbar;

    public WhenTestingUpdateMatchScore()
    {
        this.AddMudBlazorServices();
        
        _mockMatchService = Substitute.For<IMatchService>();
        _mockSnackbar = Substitute.For<ISnackbar>();

        Services.AddScoped(_ => _mockMatchService);
        Services.AddScoped(_ => _mockSnackbar);
    }

    [Fact]
    public void ItShouldHaveScoreInputFields()
    {
        // Arrange
        var matchId = Guid.NewGuid();
        var mudDialog = Substitute.For<IMudDialogInstance>();

        // Act
        var component = RenderComponent<TournamentApp.Web.Dialogs.UpdateMatchScoreDialog>(parameters => parameters
            .Add(p => p.MatchId, matchId)
            .AddCascadingValue(mudDialog));

        // Assert
        component.Should().NotBeNull();
        component.Markup.Should().Contain("Player 1 Score");
        component.Markup.Should().Contain("Player 2 Score");
    }

    [Fact]
    public void ItShouldInitializeWithMatchId()
    {
        // Arrange
        var matchId = Guid.NewGuid();
        var mudDialog = Substitute.For<IMudDialogInstance>();

        // Act
        var component = RenderComponent<TournamentApp.Web.Dialogs.UpdateMatchScoreDialog>(parameters => parameters
            .Add(p => p.MatchId, matchId)
            .AddCascadingValue(mudDialog));

        // Assert
        component.Should().NotBeNull();
    }

    [Fact]
    public async Task ItShouldCallServiceWhenFormIsSubmittedWithValidScores()
    {
        // Arrange
        var matchId = Guid.NewGuid();
        var response = new Response
        {
            ValidationErrors = new List<ValidationFailure>(),
            ErrorMessage = string.Empty
        };

        _mockMatchService.UpdateMatchScore(matchId, Arg.Any<UpdateMatchScoreViewModel>())
            .Returns(Task.FromResult(response));

        var mudDialog = Substitute.For<IMudDialogInstance>();
        var component = RenderComponent<UpdateMatchScoreDialog>(parameters => parameters
            .Add(p => p.MatchId, matchId)
            .AddCascadingValue(mudDialog));

        // Wait for component to render
        component.WaitForAssertion(() =>
        {
            component.Markup.Should().Contain("Player 1 Score");
        }, timeout: TimeSpan.FromSeconds(5));

        // Act: Find and set values on MudNumericField components
        var numericFields = component.FindComponents<MudBlazor.MudNumericField<int>>();
        numericFields.Should().HaveCount(2);
        
        // Set values on the numeric fields by finding the input and changing it
        var score1Input = numericFields[0].Find("input");
        var score2Input = numericFields[1].Find("input");
        
        score1Input.Change("10");
        score2Input.Change("5");
        
        // Wait for the component to update after changes
        component.WaitForState(() => true, TimeSpan.FromMilliseconds(300));

        var submitButton = component.Find("button[type='submit']");
        submitButton.Click();

        // Wait for async operations to complete
        component.WaitForAssertion(() =>
        {
            _mockMatchService.Received().UpdateMatchScore(matchId, Arg.Any<UpdateMatchScoreViewModel>());
        }, timeout: TimeSpan.FromSeconds(5));

        // Assert
        await _mockMatchService.Received().UpdateMatchScore(matchId, Arg.Any<UpdateMatchScoreViewModel>());
        _mockSnackbar.Received().Add("Match score updated successfully!", Severity.Success);
        mudDialog.Received().Close(Arg.Any<DialogResult>());
    }

    [Fact]
    public async Task ItShouldDisplayErrorWhenScoresAreEqual()
    {
        // Arrange
        var matchId = Guid.NewGuid();
        var mudDialog = Substitute.For<IMudDialogInstance>();

        var component = RenderComponent<UpdateMatchScoreDialog>(parameters => parameters
            .Add(p => p.MatchId, matchId)
            .AddCascadingValue(mudDialog));

        var score1Input = component.FindAll("input").FirstOrDefault(i => i.GetAttribute("aria-label")?.Contains("Player 1") == true);
        var score2Input = component.FindAll("input").FirstOrDefault(i => i.GetAttribute("aria-label")?.Contains("Player 2") == true);
        var submitButton = component.Find("button[type='submit']");

        // Act
        if (score1Input != null) score1Input.Change("10");
        if (score2Input != null) score2Input.Change("10");
        submitButton.Click();

        // Assert
        _mockSnackbar.Received().Add("Scores cannot be equal. One player must win.", Severity.Warning);
    }
}

