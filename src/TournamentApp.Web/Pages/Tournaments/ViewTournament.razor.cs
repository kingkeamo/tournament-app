using Microsoft.AspNetCore.Components;
using MudBlazor;
using TournamentApp.Shared;
using TournamentApp.Web.Contracts.Services;
using TournamentApp.Web.Dialogs;
using TournamentApp.Web.Responses;

namespace TournamentApp.Web.Pages.Tournaments;

public class ViewTournamentBase : ComponentBase
{
    [Parameter] public string TournamentId { get; set; } = string.Empty;
    [Inject] protected ITournamentService TournamentService { get; set; } = null!;
    [Inject] protected IBracketService BracketService { get; set; } = null!;
    [Inject] protected IPlayerService PlayerService { get; set; } = null!;
    [Inject] protected ISnackbar Snackbar { get; set; } = null!;
    [Inject] protected NavigationManager NavigationManager { get; set; } = null!;
    [Inject] protected IDialogService DialogService { get; set; } = null!;

    protected TournamentDto? _tournament;
    protected string? _errorMessage;
    private Guid _tournamentIdGuid;
    private Dictionary<Guid, string> _playerNames = new();
    private bool _playersLoaded;
    protected bool _isRemoving;

    protected override async Task OnInitializedAsync()
    {
        if (!Guid.TryParse(TournamentId, out _tournamentIdGuid))
        {
            _errorMessage = "Invalid tournament ID";
            return;
        }
        await LoadTournament();
        await LoadPlayers();
    }

    protected async Task LoadTournament()
    {
        _errorMessage = null;

        try
        {
            var response = await TournamentService.GetTournament(_tournamentIdGuid);

            if (response.IsFailure)
            {
                if (response.ValidationErrors.Any())
                {
                    foreach (var error in response.ValidationErrors)
                    {
                        Snackbar.Add(error.ErrorMessage, Severity.Error);
                    }
                }
                else if (!string.IsNullOrEmpty(response.ErrorMessage))
                {
                    _errorMessage = response.ErrorMessage;
                    Snackbar.Add(response.ErrorMessage, Severity.Error);
                }
                return;
            }

            _tournament = response.Data;
        }
        catch (HttpRequestException ex)
        {
            var errorMsg = $"Network error: {ex.Message}";
            _errorMessage = errorMsg;
            Snackbar.Add(errorMsg, Severity.Error);
        }
        catch (Exception ex)
        {
            var errorMsg = $"Error: {ex.Message}";
            _errorMessage = errorMsg;
            Snackbar.Add($"Error loading tournament: {errorMsg}", Severity.Error);
        }
    }

    protected Color GetStatusColor(string status)
    {
        return status switch
        {
            "Draft" => Color.Default,
            "InProgress" => Color.Info,
            "Completed" => Color.Success,
            _ => Color.Default
        };
    }

    protected async Task ShowAddPlayerDialog()
    {
        var parameters = new DialogParameters
        {
            { "TournamentId", _tournamentIdGuid }
        };
        var options = new DialogOptions 
        { 
            CloseOnEscapeKey = true, 
            MaxWidth = MaxWidth.Small, 
            FullWidth = true,
            CloseButton = true
        };

        var dialog = await DialogService.ShowAsync<AddPlayerToTournamentDialog>(null, parameters, options);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            await LoadTournament();
        }
    }

    protected async Task GenerateBracket()
    {
        try
        {
            var response = await BracketService.GenerateBracket(_tournamentIdGuid);

            if (response.IsFailure)
            {
                if (response.ValidationErrors.Any())
                {
                    foreach (var error in response.ValidationErrors)
                    {
                        Snackbar.Add(error.ErrorMessage, Severity.Error);
                    }
                }
                else if (!string.IsNullOrEmpty(response.ErrorMessage))
                {
                    Snackbar.Add(response.ErrorMessage, Severity.Error);
                }
                else
                {
                    Snackbar.Add("Error generating bracket", Severity.Error);
                }
                return;
            }

            Snackbar.Add("Bracket generated successfully!", Severity.Success);
            await LoadTournament();
        }
        catch (HttpRequestException ex)
        {
            Snackbar.Add($"Network error: {ex.Message}", Severity.Error);
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error: {ex.Message}", Severity.Error);
        }
    }

    protected void ViewBracket()
    {
        NavigationManager.NavigateTo($"/tournaments/{_tournamentIdGuid}/bracket");
    }

    private async Task LoadPlayers()
    {
        if (_playersLoaded) return;

        try
        {
            var response = await PlayerService.GetPlayers();
            if (response.IsSuccess && response.Data != null)
            {
                _playerNames = response.Data.ToDictionary(p => p.Id, p => p.Name);
                _playersLoaded = true;
                StateHasChanged();
            }
        }
        catch
        {
            // Ignore errors, will show UUID as fallback
        }
    }

    protected string GetPlayerName(Guid playerId)
    {
        if (_playerNames.TryGetValue(playerId, out var name))
        {
            return name;
        }
        return $"Player {playerId}";
    }

    protected async Task RemovePlayer(Guid playerId)
    {
        if (_isRemoving) return;

        var playerName = GetPlayerName(playerId);
        var parameters = new DialogParameters
        {
            { "Title", "Remove Player" },
            { "Message", $"Are you sure you want to remove {playerName} from this tournament?" },
            { "ConfirmText", "Remove" },
            { "CancelText", "Cancel" },
            { "ConfirmColor", Color.Error }
        };
        
        var options = new DialogOptions 
        { 
            CloseOnEscapeKey = true, 
            MaxWidth = MaxWidth.Small, 
            FullWidth = true,
            CloseButton = true
        };

        var dialog = await DialogService.ShowAsync<ConfirmDialog>("Remove Player", parameters, options);
        var result = await dialog.Result;

        if (result.Canceled)
        {
            return;
        }

        _isRemoving = true;
        StateHasChanged();

        try
        {
            var response = await TournamentService.RemovePlayerFromTournament(_tournamentIdGuid, playerId);

            if (response.IsFailure)
            {
                if (response.ValidationErrors.Any())
                {
                    foreach (var error in response.ValidationErrors)
                    {
                        Snackbar.Add(error.ErrorMessage, Severity.Error);
                    }
                }
                else if (!string.IsNullOrEmpty(response.ErrorMessage))
                {
                    Snackbar.Add(response.ErrorMessage, Severity.Error);
                }
                else
                {
                    Snackbar.Add("Error removing player", Severity.Error);
                }
            }
            else
            {
                Snackbar.Add("Player removed successfully", Severity.Success);
                await LoadTournament();
            }
        }
        catch (HttpRequestException ex)
        {
            Snackbar.Add($"Network error: {ex.Message}", Severity.Error);
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error: {ex.Message}", Severity.Error);
        }
        finally
        {
            _isRemoving = false;
            StateHasChanged();
        }
    }
}

