using Bunit;
using FluentAssertions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using NSubstitute;
using TournamentApp.Shared;
using TournamentApp.Web.Contracts.Services;
using TournamentApp.Web.Dialogs;
using TournamentApp.Web.Responses;
using TournamentApp.WebTests.Helpers;
using Xunit;

namespace TournamentApp.WebTests.Dialogs.AddPlayerToTournamentTests;

public class WhenTestingAddPlayerToTournament : TestContext
{
    private readonly IPlayerService _playerService;
    private readonly ITournamentService _tournamentService;
    private readonly ISnackbar _snackbar;

    public WhenTestingAddPlayerToTournament()
    {
        this.AddMudBlazorServices();

        _playerService = Substitute.For<IPlayerService>();
        _tournamentService = Substitute.For<ITournamentService>();
        _snackbar = Substitute.For<ISnackbar>();

        Services.AddScoped(_ => _playerService);
        Services.AddScoped(_ => _tournamentService);
        Services.AddScoped(_ => _snackbar);
    }

    [Fact]
    public async Task ItShouldLoadPlayersWhenComponentInitializes()
    {
        // Arrange
        var tournamentId = Guid.NewGuid();

        _playerService.GetPlayers().Returns(Task.FromResult(new DataResponse<IEnumerable<PlayerDto>>
        {
            Data = new[]
            {
                new PlayerDto { Id = Guid.NewGuid(), Name = "Player 1" }
            },
            ValidationErrors = new List<ValidationFailure>(),
            ErrorMessage = string.Empty
        }));

        var mudDialog = Substitute.For<IMudDialogInstance>();

        // Act
        var component = Render(builder =>
        {
            builder.OpenComponent<MudPopoverProvider>(0);
            builder.CloseComponent();
            builder.OpenComponent<CascadingValue<IMudDialogInstance>>(1);
            builder.AddAttribute(2, "Value", mudDialog);
            builder.AddAttribute(3, "ChildContent", (RenderFragment)(dialogBuilder =>
            {
                dialogBuilder.OpenComponent<AddPlayerToTournamentDialog>(0);
                dialogBuilder.AddAttribute(1, nameof(AddPlayerToTournamentDialog.TournamentId), tournamentId);
                dialogBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        // Wait for async initialization
        component.WaitForAssertion(() =>
        {
            component.Markup.Should().Contain("Select Player");
        }, timeout: TimeSpan.FromSeconds(5));

        // Assert
        await _playerService.Received(1).GetPlayers();
    }
    
    [Fact]
    public async Task ItShouldRenderPlayerSelectWhenPlayersAreLoaded()
    {
        // Arrange
        var tournamentId = Guid.NewGuid();

        _playerService.GetPlayers().Returns(Task.FromResult(new DataResponse<IEnumerable<PlayerDto>>
        {
            Data = new[]
            {
                new PlayerDto { Id = Guid.NewGuid(), Name = "Player 1" }
            },
            ValidationErrors = new List<ValidationFailure>(),
            ErrorMessage = string.Empty
        }));

        var mudDialog = Substitute.For<IMudDialogInstance>();

        // Act
        var component = Render(builder =>
        {
            builder.OpenComponent<MudPopoverProvider>(0);
            builder.CloseComponent();
            builder.OpenComponent<CascadingValue<IMudDialogInstance>>(1);
            builder.AddAttribute(2, "Value", mudDialog);
            builder.AddAttribute(3, "ChildContent", (RenderFragment)(dialogBuilder =>
            {
                dialogBuilder.OpenComponent<AddPlayerToTournamentDialog>(0);
                dialogBuilder.AddAttribute(1, nameof(AddPlayerToTournamentDialog.TournamentId), tournamentId);
                dialogBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        // Wait for dialog to render - need to wait for async initialization
        component.WaitForAssertion(() =>
        {
            component.Markup.Should().Contain("Select Player");
        }, timeout: TimeSpan.FromSeconds(5));

        // Assert
        component.Markup.Should().Contain("Select Player");
    }

    [Fact]
    public async Task ItShouldCallServiceWhenFormIsSubmitted()
    {
        // Arrange
        var tournamentId = Guid.NewGuid();
        var playerId = Guid.NewGuid();

        _playerService.GetPlayers().Returns(Task.FromResult(new DataResponse<IEnumerable<PlayerDto>>
        {
            Data = new[]
            {
                new PlayerDto { Id = playerId, Name = "Player 1" }
            },
            ValidationErrors = new List<ValidationFailure>(),
            ErrorMessage = string.Empty
        }));

        _tournamentService
            .AddPlayerToTournament(tournamentId, Arg.Any<AddPlayerToTournamentViewModel>())
            .Returns(Task.FromResult(new Response
            {
                ValidationErrors = new List<ValidationFailure>(),
                ErrorMessage = string.Empty
            }));

        var mudDialog = Substitute.For<IMudDialogInstance>();
        var component = Render(builder =>
        {
            builder.OpenComponent<MudPopoverProvider>(0);
            builder.CloseComponent();
            builder.OpenComponent<CascadingValue<IMudDialogInstance>>(1);
            builder.AddAttribute(2, "Value", mudDialog);
            builder.AddAttribute(3, "ChildContent", (RenderFragment)(dialogBuilder =>
            {
                dialogBuilder.OpenComponent<AddPlayerToTournamentDialog>(0);
                dialogBuilder.AddAttribute(1, nameof(AddPlayerToTournamentDialog.TournamentId), tournamentId);
                dialogBuilder.CloseComponent();
            }));
            builder.CloseComponent();
        });

        // Wait for component to render
        component.WaitForAssertion(() =>
        {
            component.Markup.Should().Contain("Select Player");
        }, timeout: TimeSpan.FromSeconds(5));

        // Act: Select player and submit
        var select = component.FindComponent<MudSelect<Guid>>();
        select.Instance.Value = playerId;
        select.Render();

        var submitButton = component.Find("button[type='submit']");
        submitButton.Click();

        // Wait for async operations to complete
        component.WaitForAssertion(() =>
        {
            _tournamentService.Received().AddPlayerToTournament(tournamentId, Arg.Any<AddPlayerToTournamentViewModel>());
        }, timeout: TimeSpan.FromSeconds(5));

        // Assert
        await _tournamentService.Received(1)
            .AddPlayerToTournament(tournamentId, Arg.Any<AddPlayerToTournamentViewModel>());

        _snackbar.Received(1)
            .Add("Player added to tournament successfully!", Severity.Success);
        
        mudDialog.Received().Close(Arg.Any<DialogResult>());
    }
}
