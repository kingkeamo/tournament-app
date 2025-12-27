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

namespace TournamentApp.WebTests.Dialogs.CreatePlayerTests;

public class WhenTestingCreatePlayer : TestContext
{
    private readonly IPlayerService playerService;
    private readonly ISnackbar snackbar;

    public WhenTestingCreatePlayer()
    {
        this.AddMudBlazorServices();
        
        playerService = Substitute.For<IPlayerService>();
        snackbar = Substitute.For<ISnackbar>();

        Services.AddScoped(_ => playerService);
        Services.AddScoped(_ => snackbar);
    }

    [Fact]
    public async Task ItShouldHaveNameInputField()
    {
        // Arrange
        var dialogService = Services.GetRequiredService<IDialogService>();
        var dialogProvider = RenderComponent<MudDialogProvider>();
        
        // Act - Open dialog via DialogService (MudDialog only renders when opened this way)
        var dialogReference = await dialogService.ShowAsync<CreatePlayerDialog>("");
        var component = dialogProvider;

        // Assert
        component.WaitForAssertion(() =>
        {
            var textField = component.FindComponent<MudTextField<string>>();
            textField.Should().NotBeNull();
            textField.Instance.Label.Should().Be("Player Name");
        }, timeout: TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task ItShouldCallServiceWhenFormIsSubmittedWithValidData()
    {
        // Arrange
        var newId = Guid.NewGuid();
        var response = new CreateResponse
        {
            NewId = newId,
            ValidationErrors = new List<ValidationFailure>(),
            ErrorMessage = string.Empty
        };

        playerService.AddPlayer(Arg.Any<AddPlayerViewModel>()).Returns(Task.FromResult(response));

        var dialogService = Services.GetRequiredService<IDialogService>();
        var dialogProvider = RenderComponent<MudDialogProvider>();
        
        // Act - Open dialog via DialogService
        var dialogReference = await dialogService.ShowAsync<CreatePlayerDialog>("");
        var component = dialogProvider;
        
        component.WaitForAssertion(() =>
        {
            component.Markup.Should().Contain("Player Name");
        }, timeout: TimeSpan.FromSeconds(5));
        
        var textField = component.FindComponent<MudTextField<string>>();
        var input = textField.Find("input");
        input.Change("New Player");
        
        var buttons = component.FindAll("button");
        var submitButton = buttons.FirstOrDefault(b => b.TextContent.Contains("Create Player"));
        submitButton.Should().NotBeNull();

        // Act
        submitButton!.Click();

        // Wait for async operations
        await Task.Delay(100);

        // Assert
        await playerService.Received().AddPlayer(Arg.Is<AddPlayerViewModel>(vm => vm.Name == "New Player"));
        snackbar.Received().Add("Player created successfully!", Severity.Success);
    }

    [Fact]
    public async Task ItShouldDisplayErrorWhenApiReturnsValidationErrors()
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

        playerService.AddPlayer(Arg.Any<AddPlayerViewModel>()).Returns(Task.FromResult(response));

        var dialogService = Services.GetRequiredService<IDialogService>();
        var dialogProvider = RenderComponent<MudDialogProvider>();
        
        // Act - Open dialog via DialogService
        var dialogReference = await dialogService.ShowAsync<CreatePlayerDialog>("");
        var component = dialogProvider;
        
        component.WaitForAssertion(() =>
        {
            component.Markup.Should().Contain("Player Name");
        }, timeout: TimeSpan.FromSeconds(5));
        
        var buttons = component.FindAll("button");
        var submitButton = buttons.FirstOrDefault(b => b.TextContent.Contains("Create Player"));
        submitButton.Should().NotBeNull();

        // Act - Click submit with empty field (should show client-side validation error)
        submitButton!.Click();

        // Assert - Check that validation error message is displayed in the UI
        // Dummy checkin
        component.WaitForAssertion(() =>
        {
            component.Markup.Should().Contain("Player name is required");
        }, timeout: TimeSpan.FromSeconds(5));
    }
}

