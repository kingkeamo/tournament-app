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
    public void ItShouldHaveNameInputField()
    {
        // Arrange
        var mudDialog = Substitute.For<IMudDialogInstance>();
        
        // Act
        var component = RenderComponent<TournamentApp.Web.Dialogs.CreateTournamentDialog>(
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

        _mockTournamentService.CreateTournament(Arg.Any<CreateTournamentViewModel>()).Returns(Task.FromResult(response));

        var mudDialog = Substitute.For<IMudDialogInstance>();
        var component = RenderComponent<TournamentApp.Web.Dialogs.CreateTournamentDialog>(
            builder => builder.AddCascadingValue(mudDialog));
        var nameInput = component.Find("input[type='text']");
        var submitButton = component.Find("button[type='submit']");

        // Act
        nameInput.Change("New Tournament");
        submitButton.Click();

        // Assert
        await _mockTournamentService.Received().CreateTournament(Arg.Is<CreateTournamentViewModel>(vm => vm.Name == "New Tournament"));
        _mockSnackbar.Received().Add("Tournament created successfully!", Severity.Success);
        mudDialog.Received().Close(Arg.Any<DialogResult>());
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

        var mudDialog = Substitute.For<IMudDialogInstance>();
        var component = RenderComponent<CreateTournamentDialog>(
            builder => builder.AddCascadingValue(mudDialog));
        var submitButton = component.Find("button[type='submit']");

        // Act
        submitButton.Click();

        // Assert
        _mockSnackbar.Received().Add("Tournament name is required", Severity.Error);
    }
}

