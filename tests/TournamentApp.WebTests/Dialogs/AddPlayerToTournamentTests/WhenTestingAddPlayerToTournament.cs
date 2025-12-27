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

        var dialogService = Services.GetRequiredService<IDialogService>();
        var dialogProvider = Render(builder =>
        {
            builder.OpenComponent<MudPopoverProvider>(0);
            builder.CloseComponent();
            builder.OpenComponent<MudDialogProvider>(1);
            builder.CloseComponent();
        });
        
        // Act - Open dialog via DialogService with parameters
        var parameters = new DialogParameters { { nameof(AddPlayerToTournamentDialog.TournamentId), tournamentId } };
        var dialogReference = await dialogService.ShowAsync<AddPlayerToTournamentDialog>("", parameters);
        var component = dialogProvider;

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

        var dialogService = Services.GetRequiredService<IDialogService>();
        var dialogProvider = Render(builder =>
        {
            builder.OpenComponent<MudPopoverProvider>(0);
            builder.CloseComponent();
            builder.OpenComponent<MudDialogProvider>(1);
            builder.CloseComponent();
        });
        
        // Act - Open dialog via DialogService with parameters
        var parameters = new DialogParameters { { nameof(AddPlayerToTournamentDialog.TournamentId), tournamentId } };
        var dialogReference = await dialogService.ShowAsync<AddPlayerToTournamentDialog>("", parameters);
        var component = dialogProvider;

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

        var dialogService = Services.GetRequiredService<IDialogService>();
        var dialogProvider = Render(builder =>
        {
            builder.OpenComponent<MudPopoverProvider>(0);
            builder.CloseComponent();
            builder.OpenComponent<MudDialogProvider>(1);
            builder.CloseComponent();
        });
        
        // Act - Open dialog via DialogService with parameters
        var parameters = new DialogParameters { { nameof(AddPlayerToTournamentDialog.TournamentId), tournamentId } };
        var dialogReference = await dialogService.ShowAsync<AddPlayerToTournamentDialog>("", parameters);
        var component = dialogProvider;

        // Wait for component to render
        component.WaitForAssertion(() =>
        {
            component.Markup.Should().Contain("Select Player");
        }, timeout: TimeSpan.FromSeconds(5));

        // Act: Select player and submit
        var select = component.FindComponent<MudSelect<Guid>>();
        select.Instance.Value = playerId;
        select.Render();

        var buttons = component.FindAll("button");
        var submitButton = buttons.FirstOrDefault(b => b.TextContent.Contains("Add Player"));
        submitButton.Should().NotBeNull();
        submitButton!.Click();

        // Wait for async operations
        await Task.Delay(100);

        // Assert
        await _tournamentService.Received(1)
            .AddPlayerToTournament(tournamentId, Arg.Any<AddPlayerToTournamentViewModel>());

        _snackbar.Received(1)
            .Add("Player added to tournament successfully!", Severity.Success);
    }
}
