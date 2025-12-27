using Bunit;
using FluentAssertions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Components;
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
    public async Task ItShouldHaveScoreInputFields()
    {
        // Arrange
        var matchId = Guid.NewGuid();
        var dialogService = Services.GetRequiredService<IDialogService>();
        var dialogProvider = RenderComponent<MudDialogProvider>();
        
        // Act - Open dialog via DialogService with parameters
        var parameters = new DialogParameters { { nameof(UpdateMatchScoreDialog.MatchId), matchId } };
        var dialogReference = await dialogService.ShowAsync<UpdateMatchScoreDialog>("", parameters);
        var component = dialogProvider;

        // Assert
        component.WaitForAssertion(() =>
        {
            component.Markup.Should().Contain("Player 1 Score");
            component.Markup.Should().Contain("Player 2 Score");
        }, timeout: TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task ItShouldInitializeWithMatchId()
    {
        // Arrange
        var matchId = Guid.NewGuid();
        var dialogService = Services.GetRequiredService<IDialogService>();
        var dialogProvider = RenderComponent<MudDialogProvider>();
        
        // Act - Open dialog via DialogService with parameters
        var parameters = new DialogParameters { { nameof(UpdateMatchScoreDialog.MatchId), matchId } };
        var dialogReference = await dialogService.ShowAsync<UpdateMatchScoreDialog>("", parameters);
        var component = dialogProvider;

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

        var dialogService = Services.GetRequiredService<IDialogService>();
        var dialogProvider = RenderComponent<MudDialogProvider>();
        
        // Act - Open dialog via DialogService with parameters
        var parameters = new DialogParameters { { nameof(UpdateMatchScoreDialog.MatchId), matchId } };
        var dialogReference = await dialogService.ShowAsync<UpdateMatchScoreDialog>("", parameters);
        var component = dialogProvider;

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

        var buttons = component.FindAll("button");
        var submitButton = buttons.FirstOrDefault(b => b.TextContent.Contains("Update Score"));
        submitButton.Should().NotBeNull();
        submitButton!.Click();

        // Wait for async operations
        await Task.Delay(100);

        // Assert
        await _mockMatchService.Received().UpdateMatchScore(matchId, Arg.Any<UpdateMatchScoreViewModel>());
        _mockSnackbar.Received().Add("Match score updated successfully!", Severity.Success);
    }

    [Fact]
    public async Task ItShouldDisplayErrorWhenScoresAreEqual()
    {
        // Arrange
        var matchId = Guid.NewGuid();
        var dialogService = Services.GetRequiredService<IDialogService>();
        var dialogProvider = RenderComponent<MudDialogProvider>();
        
        // Act - Open dialog via DialogService with parameters
        var parameters = new DialogParameters { { nameof(UpdateMatchScoreDialog.MatchId), matchId } };
        var dialogReference = await dialogService.ShowAsync<UpdateMatchScoreDialog>("", parameters);
        var component = dialogProvider;
        
        component.WaitForAssertion(() =>
        {
            component.Markup.Should().Contain("Player 1 Score");
        }, timeout: TimeSpan.FromSeconds(5));

        component.WaitForAssertion(() =>
        {
            component.Markup.Should().Contain("Player 1 Score");
        }, timeout: TimeSpan.FromSeconds(5));

        var numericFields = component.FindComponents<MudNumericField<int>>();
        numericFields.Should().HaveCount(2);
        
        var score1Input = numericFields[0].Find("input");
        var score2Input = numericFields[1].Find("input");
        
        score1Input.Change("10");
        score2Input.Change("10");
        
        var buttons = component.FindAll("button");
        var submitButton = buttons.FirstOrDefault(b => b.TextContent.Contains("Update Score"));
        submitButton.Should().NotBeNull();

        // Act
        submitButton!.Click();

        // Wait for async operations
        await Task.Delay(100);

        // Assert
        _mockSnackbar.Received().Add("Scores cannot be equal. One player must win.", Severity.Warning);
    }
}

