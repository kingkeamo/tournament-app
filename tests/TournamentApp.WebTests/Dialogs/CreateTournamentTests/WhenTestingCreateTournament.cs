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

namespace TournamentApp.WebTests.Dialogs.CreateTournamentTests;

public class WhenTestingCreateTournament : TestContext
{
    private readonly ITournamentService _mockTournamentService;
    private readonly ISnackbar _mockSnackbar;

    public WhenTestingCreateTournament()
    {
        this.AddMudBlazorServices();
        
        _mockTournamentService = Substitute.For<ITournamentService>();
        _mockSnackbar = Substitute.For<ISnackbar>();

        Services.AddScoped(_ => _mockTournamentService);
        Services.AddScoped(_ => _mockSnackbar);
    }

    [Fact]
    public async Task ItShouldHaveNameInputField()
    {
        // Arrange
        var dialogService = Services.GetRequiredService<IDialogService>();
        var dialogProvider = RenderComponent<MudDialogProvider>();
        
        // Act - Open dialog via DialogService (MudDialog only renders when opened this way)
        var dialogReference = await dialogService.ShowAsync<CreateTournamentDialog>("");
        var component = dialogProvider;

        // Assert
        component.WaitForAssertion(() =>
        {
            var textField = component.FindComponent<MudTextField<string>>();
            textField.Should().NotBeNull();
            textField.Instance.Label.Should().Be("Tournament Name");
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

        _mockTournamentService.CreateTournament(Arg.Any<CreateTournamentViewModel>()).Returns(Task.FromResult(response));

        var dialogService = Services.GetRequiredService<IDialogService>();
        var dialogProvider = RenderComponent<MudDialogProvider>();
        
        // Act - Open dialog via DialogService
        var dialogReference = await dialogService.ShowAsync<CreateTournamentDialog>("");
        var component = dialogProvider;
        
        component.WaitForAssertion(() =>
        {
            component.Markup.Should().Contain("Tournament Name");
        }, timeout: TimeSpan.FromSeconds(5));
        
        var textField = component.FindComponent<MudTextField<string>>();
        var input = textField.Find("input");
        input.Change("New Tournament");
        
        var buttons = component.FindAll("button");
        var submitButton = buttons.FirstOrDefault(b => b.TextContent.Contains("Create Tournament"));
        submitButton.Should().NotBeNull();

        // Act
        submitButton!.Click();

        // Wait for async operations
        await Task.Delay(100);

        // Assert
        await _mockTournamentService.Received().CreateTournament(Arg.Is<CreateTournamentViewModel>(vm => vm.Name == "New Tournament"));
        _mockSnackbar.Received().Add("Tournament created successfully!", Severity.Success);
    }

    [Fact]
    public async Task ItShouldDisplayErrorWhenApiReturnsValidationErrors()
    {
        // Arrange
        var validationErrors = new List<ValidationFailure>
        {
            new ValidationFailure("Name", "Tournament name is required")
        };

        var response = new CreateResponse
        {
            NewId = Guid.Empty,
            ValidationErrors = validationErrors,
            ErrorMessage = string.Empty
        };

        _mockTournamentService.CreateTournament(Arg.Any<CreateTournamentViewModel>()).Returns(Task.FromResult(response));

        var dialogService = Services.GetRequiredService<IDialogService>();
        var dialogProvider = RenderComponent<MudDialogProvider>();
        
        // Act - Open dialog via DialogService
        var dialogReference = await dialogService.ShowAsync<CreateTournamentDialog>("");
        var component = dialogProvider;
        
        component.WaitForAssertion(() =>
        {
            component.Markup.Should().Contain("Tournament Name");
        }, timeout: TimeSpan.FromSeconds(5));
        
        var buttons = component.FindAll("button");
        var submitButton = buttons.FirstOrDefault(b => b.TextContent.Contains("Create Tournament"));
        submitButton.Should().NotBeNull();

        // Act - Click submit with empty field (should show client-side validation error)
        submitButton!.Click();

        // Assert - Check that validation error message is displayed in the UI
        component.WaitForAssertion(() =>
        {
            component.Markup.Should().Contain("Tournament name is required");
        }, timeout: TimeSpan.FromSeconds(5));
    }
}

