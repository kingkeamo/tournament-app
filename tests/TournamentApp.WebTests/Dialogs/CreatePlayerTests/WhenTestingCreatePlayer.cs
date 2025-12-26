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
    public void ItShouldHaveNameInputField()
    {
        // Arrange
        var mudDialog = Substitute.For<IMudDialogInstance>();
        
        // Act
        var component = RenderComponent<TournamentApp.Web.Dialogs.CreatePlayerDialog>(
            builder => builder.AddCascadingValue(mudDialog));

        // Assert
        component.Find("input[type='text']").Should().NotBeNull();
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

        var mudDialog = Substitute.For<IMudDialogInstance>();
        var component = RenderComponent<TournamentApp.Web.Dialogs.CreatePlayerDialog>(
            builder => builder.AddCascadingValue(mudDialog));
        var nameInput = component.Find("input[type='text']");
        var submitButton = component.Find("button[type='submit']");

        // Act
        nameInput.Change("New Player");
        submitButton.Click();

        // Assert
        await playerService.Received().AddPlayer(Arg.Is<AddPlayerViewModel>(vm => vm.Name == "New Player"));
        snackbar.Received().Add("Player created successfully!", Severity.Success);
        mudDialog.Received().Close(Arg.Any<DialogResult>());
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

        var mudDialog = Substitute.For<IMudDialogInstance>();
        var component = RenderComponent<CreatePlayerDialog>(
            builder => builder.AddCascadingValue(mudDialog));
        var submitButton = component.Find("button[type='submit']");

        // Act
        submitButton.Click();

        // Assert
        snackbar.Received().Add("Player name is required", Severity.Error);
    }
}

