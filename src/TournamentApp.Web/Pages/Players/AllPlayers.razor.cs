using FluentValidation.Results;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using TournamentApp.Shared;
using TournamentApp.Web.Contracts.Services;
using TournamentApp.Web.Dialogs;

namespace TournamentApp.Web.Pages.Players;

public class AllPlayersBase : ComponentBase
{
    [Inject] protected IPlayerService PlayerService { get; set; } = null!;
    [Inject] protected ISnackbar Snackbar { get; set; } = null!;
    [Inject] protected IDialogService DialogService { get; set; } = null!;

    protected List<PlayerDto>? _players;
    protected string? _errorMessage;
    protected IList<ValidationFailure>? _validationErrors;

    protected override async Task OnInitializedAsync()
    {
        await LoadPlayers();
    }

    protected async Task LoadPlayers()
    {
        _errorMessage = null;
        _validationErrors = null;

        try
        {
            var response = await PlayerService.GetPlayers();

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
                _players = new List<PlayerDto>();
                return;
            }

            _players = response.Data?.ToList() ?? new List<PlayerDto>();
        }
        catch (HttpRequestException ex)
        {
            var errorMsg = $"Network error: {ex.Message}";
            _errorMessage = errorMsg;
            Snackbar.Add(errorMsg, Severity.Error);
            _players = new List<PlayerDto>();
        }
        catch (Exception ex)
        {
            var errorMsg = $"Error: {ex.Message}";
            _errorMessage = errorMsg;
            Snackbar.Add($"Error loading players: {errorMsg}", Severity.Error);
            _players = new List<PlayerDto>();
        }
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

        var dialog = await DialogService.ShowAsync<CreatePlayerDialog>(null, parameters, options);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            await LoadPlayers();
        }
    }
}

