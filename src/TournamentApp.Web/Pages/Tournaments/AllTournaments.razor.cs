using FluentValidation.Results;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using TournamentApp.Shared;
using TournamentApp.Web.Contracts.Services;
using TournamentApp.Web.Dialogs;

namespace TournamentApp.Web.Pages.Tournaments;

public class AllTournamentsBase : ComponentBase
{
    [Inject] protected ITournamentService TournamentService { get; set; } = null!;
    [Inject] protected IBracketService BracketService { get; set; } = null!;
    [Inject] protected ISnackbar Snackbar { get; set; } = null!;
    [Inject] protected NavigationManager NavigationManager { get; set; } = null!;
    [Inject] protected IDialogService DialogService { get; set; } = null!;

    protected List<TournamentDto>? _tournaments;
    protected string? _errorMessage;
    protected IList<ValidationFailure>? _validationErrors;

    protected override async Task OnInitializedAsync()
    {
        await LoadTournaments();
    }

    protected async Task LoadTournaments()
    {
        _errorMessage = null;
        _validationErrors = null;

        try
        {
            var response = await TournamentService.GetTournaments();

            if (response.IsFailure)
            {
                if (response.ValidationErrors.Any())
                {
                    _validationErrors = response.ValidationErrors;
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
                _tournaments = new List<TournamentDto>();
                return;
            }

            _tournaments = response.Data?.ToList() ?? new List<TournamentDto>();
        }
        catch (HttpRequestException ex)
        {
            var errorMsg = $"Network error: {ex.Message}";
            _errorMessage = errorMsg;
            Snackbar.Add(errorMsg, Severity.Error);
            _tournaments = new List<TournamentDto>();
        }
        catch (Exception ex)
        {
            var errorMsg = $"Error: {ex.Message}";
            _errorMessage = errorMsg;
            Snackbar.Add($"Error loading tournaments: {errorMsg}", Severity.Error);
            _tournaments = new List<TournamentDto>();
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

    protected async Task ShowCreateDialog()
    {
        var parameters = new DialogParameters();
        var options = new DialogOptions 
        { 
            CloseOnEscapeKey = true, 
            MaxWidth = MaxWidth.Small, 
            FullWidth = true,
            CloseButton = true
        };

        var dialog = await DialogService.ShowAsync<CreateTournamentDialog>(null, parameters, options);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            await LoadTournaments();
        }
    }

    protected void ViewTournament(Guid tournamentId)
    {
        NavigationManager.NavigateTo($"tournaments/{tournamentId}");
    }

    protected async Task GenerateBracket(Guid tournamentId)
    {
        try
        {
            var response = await BracketService.GenerateBracket(tournamentId);

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
            await LoadTournaments();
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
}

