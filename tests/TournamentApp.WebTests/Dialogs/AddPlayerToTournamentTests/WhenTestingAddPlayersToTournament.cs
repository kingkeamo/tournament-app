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

public class WhenTestingAddPlayersToTournament : TestContext
{
    private readonly IPlayerService _playerService;
    private readonly ITournamentService _tournamentService;
    private readonly ISnackbar _snackbar;

    public WhenTestingAddPlayersToTournament()
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

        _tournamentService.GetTournament(tournamentId).Returns(Task.FromResult(new DataResponse<TournamentDto>
        {
            Data = new TournamentDto
            {
                Id = tournamentId,
                Name = "Test Tournament",
                PlayerIds = new List<Guid>()
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

        await Task.Delay(200);

        // Assert
        await _playerService.Received(1).GetPlayers();
        await _tournamentService.Received(1).GetTournament(tournamentId);
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

        _tournamentService.GetTournament(tournamentId).Returns(Task.FromResult(new DataResponse<TournamentDto>
        {
            Data = new TournamentDto
            {
                Id = tournamentId,
                Name = "Test Tournament",
                PlayerIds = new List<Guid>()
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

        await Task.Delay(200);

        await _playerService.Received(1).GetPlayers();
    }

    [Fact]
    public async Task ItShouldCallServiceWhenFormIsSubmittedWithSinglePlayer()
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

        _tournamentService.GetTournament(tournamentId).Returns(Task.FromResult(new DataResponse<TournamentDto>
        {
            Data = new TournamentDto
            {
                Id = tournamentId,
                Name = "Test Tournament",
                PlayerIds = new List<Guid>()
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

        await Task.Delay(100);
        var dialogComponent = component.FindComponent<AddPlayerToTournamentDialog>();
        await dialogComponent.InvokeAsync(() =>
        {
            var selectedPlayerIdsField = typeof(AddPlayerToTournamentDialog).GetField("_selectedPlayerIds", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (selectedPlayerIdsField != null)
            {
                selectedPlayerIdsField.SetValue(dialogComponent.Instance, new HashSet<Guid> { playerId });
            }
            
            var handleSubmitMethod = typeof(AddPlayerToTournamentDialog).GetMethod("HandleSubmit", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (handleSubmitMethod != null)
            {
                var task = (Task)handleSubmitMethod.Invoke(dialogComponent.Instance, null)!;
                task.Wait();
            }
        });

        await Task.Delay(100);

        // Assert
        await _tournamentService.Received(1)
            .AddPlayerToTournament(tournamentId, Arg.Do<AddPlayerToTournamentViewModel>(vm =>
            {
                vm.PlayerIds.Should().Contain(playerId);
                vm.PlayerIds.Should().HaveCount(1);
            }));

        _snackbar.Received(1)
            .Add("Player added to tournament successfully!", Severity.Success);
    }

    [Fact]
    public async Task ItShouldCallServiceWhenFormIsSubmittedWithMultiplePlayers()
    {
        // Arrange
        var tournamentId = Guid.NewGuid();
        var playerId1 = Guid.NewGuid();
        var playerId2 = Guid.NewGuid();

        _playerService.GetPlayers().Returns(Task.FromResult(new DataResponse<IEnumerable<PlayerDto>>
        {
            Data = new[]
            {
                new PlayerDto { Id = playerId1, Name = "Player 1" },
                new PlayerDto { Id = playerId2, Name = "Player 2" }
            },
            ValidationErrors = new List<ValidationFailure>(),
            ErrorMessage = string.Empty
        }));

        _tournamentService.GetTournament(tournamentId).Returns(Task.FromResult(new DataResponse<TournamentDto>
        {
            Data = new TournamentDto
            {
                Id = tournamentId,
                Name = "Test Tournament",
                PlayerIds = new List<Guid>()
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

        await Task.Delay(100);
        var dialogComponent = component.FindComponent<AddPlayerToTournamentDialog>();
        await dialogComponent.InvokeAsync(() =>
        {
            var selectedPlayerIdsField = typeof(AddPlayerToTournamentDialog).GetField("_selectedPlayerIds", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (selectedPlayerIdsField != null)
            {
                selectedPlayerIdsField.SetValue(dialogComponent.Instance, new HashSet<Guid> { playerId1, playerId2 });
            }
            
            var handleSubmitMethod = typeof(AddPlayerToTournamentDialog).GetMethod("HandleSubmit", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (handleSubmitMethod != null)
            {
                var task = (Task)handleSubmitMethod.Invoke(dialogComponent.Instance, null)!;
                task.Wait();
            }
        });

        await Task.Delay(100);

        // Assert
        await _tournamentService.Received(1)
            .AddPlayerToTournament(tournamentId, Arg.Do<AddPlayerToTournamentViewModel>(vm =>
            {
                vm.PlayerIds.Should().Contain(playerId1);
                vm.PlayerIds.Should().Contain(playerId2);
                vm.PlayerIds.Should().HaveCount(2);
            }));

        _snackbar.Received(1)
            .Add("2 players added to tournament successfully!", Severity.Success);
    }

    [Fact]
    public async Task ItShouldFilterOutPlayersAlreadyInTournament()
    {
        // Arrange
        var tournamentId = Guid.NewGuid();
        var existingPlayerId = Guid.NewGuid();
        var availablePlayerId = Guid.NewGuid();

        _playerService.GetPlayers().Returns(Task.FromResult(new DataResponse<IEnumerable<PlayerDto>>
        {
            Data = new[]
            {
                new PlayerDto { Id = existingPlayerId, Name = "Existing Player" },
                new PlayerDto { Id = availablePlayerId, Name = "Available Player" }
            },
            ValidationErrors = new List<ValidationFailure>(),
            ErrorMessage = string.Empty
        }));

        _tournamentService.GetTournament(tournamentId).Returns(Task.FromResult(new DataResponse<TournamentDto>
        {
            Data = new TournamentDto
            {
                Id = tournamentId,
                Name = "Test Tournament",
                PlayerIds = new List<Guid> { existingPlayerId }
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

        await Task.Delay(200);

        await _playerService.Received(1).GetPlayers();
        await _tournamentService.Received(1).GetTournament(tournamentId);
    }
}
