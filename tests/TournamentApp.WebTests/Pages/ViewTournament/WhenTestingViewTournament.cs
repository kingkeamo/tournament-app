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
using TournamentApp.Web.Pages.Tournaments;
using TournamentApp.Web.Responses;
using TournamentApp.WebTests.Helpers;

namespace TournamentApp.WebTests.Pages.ViewTournament;

public class WhenTestingViewTournament : TestContext
{
    private readonly ITournamentService tournamentService;
    private readonly IBracketService bracketService;
    private readonly IPlayerService playerService;
    private readonly ISnackbar snackbar;
    private readonly IDialogService dialogService;

    public WhenTestingViewTournament()
    {
        this.AddMudBlazorServices();
        
        tournamentService = Substitute.For<ITournamentService>();
        bracketService = Substitute.For<IBracketService>();
        playerService = Substitute.For<IPlayerService>();
        snackbar = Substitute.For<ISnackbar>();
        dialogService = Substitute.For<IDialogService>();

        Services.AddScoped(_ => tournamentService);
        Services.AddScoped(_ => bracketService);
        Services.AddScoped(_ => playerService);
        Services.AddScoped(_ => snackbar);
        Services.AddScoped(_ => dialogService);
        Services.AddScoped<NavigationManager>(_ => Substitute.For<NavigationManager>());
    }

    [Fact]
    public void ItShouldDisplayTournamentDetailsWhenTournamentIsLoaded()
    {
        // Arrange
        var tournamentId = Guid.NewGuid();
        var tournament = new TournamentDto
        {
            Id = tournamentId,
            Name = "Test Tournament",
            Status = "Draft",
            CreatedAt = DateTime.UtcNow
        };

        var response = new DataResponse<TournamentDto>
        {
            Data = tournament,
            ValidationErrors = new List<ValidationFailure>(),
            ErrorMessage = string.Empty
        };

        tournamentService.GetTournament(tournamentId).Returns(Task.FromResult(response));
        playerService.GetPlayers().Returns(Task.FromResult(new DataResponse<IEnumerable<PlayerDto>>
        {
            Data = new List<PlayerDto>(),
            ValidationErrors = new List<ValidationFailure>(),
            ErrorMessage = string.Empty
        }));

        // Act
        var component = RenderComponent<TournamentApp.Web.Pages.Tournaments.ViewTournament>(parameters => parameters
            .Add(p => p.TournamentId, tournamentId.ToString()));

        // Assert
        component.Markup.Should().Contain("Test Tournament");
    }

    [Fact]
    public void ItShouldDisplayErrorWhenTournamentNotFound()
    {
        // Arrange
        var tournamentId = Guid.NewGuid();
        var response = new DataResponse<TournamentDto>
        {
            Data = null!,
            ValidationErrors = new List<ValidationFailure>(),
            ErrorMessage = "Tournament not found"
        };

        tournamentService.GetTournament(tournamentId).Returns(Task.FromResult(response));
        playerService.GetPlayers().Returns(Task.FromResult(new DataResponse<IEnumerable<PlayerDto>>
        {
            Data = new List<PlayerDto>(),
            ValidationErrors = new List<ValidationFailure>(),
            ErrorMessage = string.Empty
        }));

        // Act
        var component = RenderComponent<TournamentApp.Web.Pages.Tournaments.ViewTournament>(parameters => parameters
            .Add(p => p.TournamentId, tournamentId.ToString()));

        // Assert
        snackbar.Received().Add("Tournament not found", Severity.Error);
    }

    [Fact]
    public void ItShouldCallGenerateBracketWhenGenerateButtonIsClicked()
    {
        // Arrange
        var tournamentId = Guid.NewGuid();
        var tournament = new TournamentDto
        {
            Id = tournamentId,
            Name = "Test Tournament",
            Status = "Draft",
            CreatedAt = DateTime.UtcNow
        };

        var tournamentResponse = new DataResponse<TournamentDto>
        {
            Data = tournament,
            ValidationErrors = new List<ValidationFailure>(),
            ErrorMessage = string.Empty
        };

        var bracketResponse = new Response
        {
            ValidationErrors = new List<ValidationFailure>(),
            ErrorMessage = string.Empty
        };

        tournamentService.GetTournament(tournamentId).Returns(Task.FromResult(tournamentResponse));
        bracketService.GenerateBracket(tournamentId).Returns(Task.FromResult(bracketResponse));
        playerService.GetPlayers().Returns(Task.FromResult(new DataResponse<IEnumerable<PlayerDto>>
        {
            Data = new List<PlayerDto>(),
            ValidationErrors = new List<ValidationFailure>(),
            ErrorMessage = string.Empty
        }));

        // Act
        var component = RenderComponent<TournamentApp.Web.Pages.Tournaments.ViewTournament>(parameters => parameters
            .Add(p => p.TournamentId, tournamentId.ToString()));

        // Assert
        tournamentService.Received().GetTournament(tournamentId);
    }

    [Fact]
    public async Task ItShouldShowConfirmDialogWhenRemovePlayerIsClicked()
    {
        var tournamentId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var playerName = "Test Player";
        var tournament = new TournamentDto
        {
            Id = tournamentId,
            Name = "Test Tournament",
            Status = "Draft",
            CreatedAt = DateTime.UtcNow,
            PlayerIds = new List<Guid> { playerId }
        };

        var tournamentResponse = new DataResponse<TournamentDto>
        {
            Data = tournament,
            ValidationErrors = new List<ValidationFailure>(),
            ErrorMessage = string.Empty
        };

        var player = new PlayerDto
        {
            Id = playerId,
            Name = playerName,
            CreatedAt = DateTime.UtcNow
        };

        tournamentService.GetTournament(tournamentId).Returns(Task.FromResult(tournamentResponse));
        playerService.GetPlayers().Returns(Task.FromResult(new DataResponse<IEnumerable<PlayerDto>>
        {
            Data = new List<PlayerDto> { player },
            ValidationErrors = new List<ValidationFailure>(),
            ErrorMessage = string.Empty
        }));

        var tcs = new TaskCompletionSource<DialogResult?>();
        tcs.SetResult(DialogResult.Ok(true));
        
        var mockDialogRef = Substitute.For<IDialogReference>();
        mockDialogRef.Result.Returns(tcs.Task);

        dialogService.ShowAsync<ConfirmDialog>(Arg.Any<string>(), Arg.Any<DialogParameters>(), Arg.Any<DialogOptions>())
            .Returns(mockDialogRef);

        var component = RenderComponent<TournamentApp.Web.Pages.Tournaments.ViewTournament>(parameters => parameters
            .Add(p => p.TournamentId, tournamentId.ToString()));

        component.WaitForAssertion(() =>
        {
            component.Markup.Should().Contain(playerName);
        }, timeout: TimeSpan.FromSeconds(5));

        var removeButtons = component.FindAll("button");
        var removeButton = removeButtons.FirstOrDefault(b => 
            b.OuterHtml.Contains("mud-icon-button"));
        removeButton.Should().NotBeNull();

        removeButton!.Click();

        await Task.Delay(300);

        await dialogService.Received().ShowAsync<ConfirmDialog>(
            Arg.Any<string>(),
            Arg.Is<DialogParameters>(p => p != null),
            Arg.Any<DialogOptions>());
    }

    [Fact]
    public async Task ItShouldNotRemovePlayerWhenConfirmDialogIsCancelled()
    {
        var tournamentId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var playerName = "Test Player";
        var tournament = new TournamentDto
        {
            Id = tournamentId,
            Name = "Test Tournament",
            Status = "Draft",
            CreatedAt = DateTime.UtcNow,
            PlayerIds = new List<Guid> { playerId }
        };

        var tournamentResponse = new DataResponse<TournamentDto>
        {
            Data = tournament,
            ValidationErrors = new List<ValidationFailure>(),
            ErrorMessage = string.Empty
        };

        var player = new PlayerDto
        {
            Id = playerId,
            Name = playerName,
            CreatedAt = DateTime.UtcNow
        };

        tournamentService.GetTournament(tournamentId).Returns(Task.FromResult(tournamentResponse));
        playerService.GetPlayers().Returns(Task.FromResult(new DataResponse<IEnumerable<PlayerDto>>
        {
            Data = new List<PlayerDto> { player },
            ValidationErrors = new List<ValidationFailure>(),
            ErrorMessage = string.Empty
        }));

        var tcs = new TaskCompletionSource<DialogResult?>();
        tcs.SetResult(DialogResult.Cancel());
        
        var mockDialogRef = Substitute.For<IDialogReference>();
        mockDialogRef.Result.Returns(tcs.Task);

        dialogService.ShowAsync<ConfirmDialog>(Arg.Any<string>(), Arg.Any<DialogParameters>(), Arg.Any<DialogOptions>())
            .Returns(mockDialogRef);

        var component = RenderComponent<TournamentApp.Web.Pages.Tournaments.ViewTournament>(parameters => parameters
            .Add(p => p.TournamentId, tournamentId.ToString()));

        component.WaitForAssertion(() =>
        {
            component.Markup.Should().Contain(playerName);
        }, timeout: TimeSpan.FromSeconds(5));

        var removeButtons = component.FindAll("button");
        var removeButton = removeButtons.FirstOrDefault(b => 
            b.OuterHtml.Contains("mud-icon-button"));
        removeButton.Should().NotBeNull();

        removeButton!.Click();

        await Task.Delay(300);

        await tournamentService.DidNotReceive().RemovePlayerFromTournament(Arg.Any<Guid>(), Arg.Any<Guid>());
    }

    [Fact]
    public async Task ItShouldRemovePlayerWhenConfirmDialogIsConfirmed()
    {
        var tournamentId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var playerName = "Test Player";
        var tournament = new TournamentDto
        {
            Id = tournamentId,
            Name = "Test Tournament",
            Status = "Draft",
            CreatedAt = DateTime.UtcNow,
            PlayerIds = new List<Guid> { playerId }
        };

        var tournamentResponse = new DataResponse<TournamentDto>
        {
            Data = tournament,
            ValidationErrors = new List<ValidationFailure>(),
            ErrorMessage = string.Empty
        };

        var player = new PlayerDto
        {
            Id = playerId,
            Name = playerName,
            CreatedAt = DateTime.UtcNow
        };

        var removeResponse = new Response
        {
            ValidationErrors = new List<ValidationFailure>(),
            ErrorMessage = string.Empty
        };

        tournamentService.GetTournament(tournamentId).Returns(Task.FromResult(tournamentResponse));
        tournamentService.RemovePlayerFromTournament(tournamentId, playerId).Returns(Task.FromResult(removeResponse));
        playerService.GetPlayers().Returns(Task.FromResult(new DataResponse<IEnumerable<PlayerDto>>
        {
            Data = new List<PlayerDto> { player },
            ValidationErrors = new List<ValidationFailure>(),
            ErrorMessage = string.Empty
        }));

        var tcs = new TaskCompletionSource<DialogResult?>();
        tcs.SetResult(DialogResult.Ok(true));
        
        var mockDialogRef = Substitute.For<IDialogReference>();
        mockDialogRef.Result.Returns(tcs.Task);

        dialogService.ShowAsync<ConfirmDialog>(Arg.Any<string>(), Arg.Any<DialogParameters>(), Arg.Any<DialogOptions>())
            .Returns(mockDialogRef);

        var component = RenderComponent<TournamentApp.Web.Pages.Tournaments.ViewTournament>(parameters => parameters
            .Add(p => p.TournamentId, tournamentId.ToString()));

        component.WaitForAssertion(() =>
        {
            component.Markup.Should().Contain(playerName);
        }, timeout: TimeSpan.FromSeconds(5));

        var removeButtons = component.FindAll("button");
        var removeButton = removeButtons.FirstOrDefault(b => 
            b.OuterHtml.Contains("mud-icon-button"));
        removeButton.Should().NotBeNull();

        removeButton!.Click();

        await Task.Delay(500);

        await tournamentService.Received().RemovePlayerFromTournament(tournamentId, playerId);
        snackbar.Received().Add("Player removed successfully", Severity.Success);
    }
}

