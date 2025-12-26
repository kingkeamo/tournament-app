using Microsoft.AspNetCore.Components;
using MudBlazor;
using TournamentApp.Shared;
using TournamentApp.Web.Contracts.Services;
using TournamentApp.Web.Responses;

namespace TournamentApp.Web.Pages.Bracket;

public class ViewBracketBase : ComponentBase
{
    [Parameter] public string TournamentId { get; set; } = string.Empty;
    [Inject] protected IBracketService BracketService { get; set; } = null!;
    [Inject] protected ITournamentService TournamentService { get; set; } = null!;
    [Inject] protected ISnackbar Snackbar { get; set; } = null!;

    protected BracketDto? _bracket;
    protected string? _errorMessage;
    protected string? _tournamentName;
    private Guid _tournamentIdGuid;

    protected override async Task OnInitializedAsync()
    {
        if (!Guid.TryParse(TournamentId, out _tournamentIdGuid))
        {
            _errorMessage = "Invalid tournament ID";
            return;
        }
        await LoadTournamentName();
        await LoadBracket();
    }

    protected async Task LoadTournamentName()
    {
        try
        {
            var response = await TournamentService.GetTournament(_tournamentIdGuid);
            if (response.IsSuccess && response.Data != null)
            {
                _tournamentName = response.Data.Name;
            }
        }
        catch
        {
            // Ignore error for tournament name
        }
    }

    protected async Task LoadBracket()
    {
        _errorMessage = null;

        try
        {
            var response = await BracketService.GetBracket(_tournamentIdGuid);

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

            _bracket = response.Data;
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
            Snackbar.Add($"Error loading bracket: {errorMsg}", Severity.Error);
        }
    }

    protected async Task HandleScoreUpdate()
    {
        await LoadBracket();
        StateHasChanged();
    }
}





