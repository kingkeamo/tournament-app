using FluentValidation.Results;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using TournamentApp.Shared;
using TournamentApp.Web.Contracts.Services;
using TournamentApp.Web.Responses;

namespace TournamentApp.Web.Dialogs;

public partial class AddPlayerToTournamentDialog : ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; } = null!;
    [Parameter] public Guid TournamentId { get; set; }
    [Inject] protected IPlayerService PlayerService { get; set; } = null!;
    [Inject] protected ITournamentService TournamentService { get; set; } = null!;
    [Inject] protected ISnackbar Snackbar { get; set; } = null!;

    private AddPlayerToTournamentViewModel _viewModel = new();
    private List<PlayerDto>? _players;
    private bool _isSaving;

    protected override async Task OnInitializedAsync()
    {
        _viewModel.TournamentId = TournamentId;
        await LoadPlayers();
    }

    private async Task LoadPlayers()
    {
        try
        {
            var response = await PlayerService.GetPlayers();
            if (response.IsSuccess && response.Data != null)
            {
                _players = response.Data.ToList();
            }
            else
            {
                _players = new List<PlayerDto>();
            }
        }
        catch
        {
            _players = new List<PlayerDto>();
        }
        
        StateHasChanged();
    }

    private async Task HandleValidSubmit()
    {
        _isSaving = true;
        
        try
        {
            var response = await TournamentService.AddPlayerToTournament(TournamentId, _viewModel);

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
                _isSaving = false;
                return;
            }

            Snackbar.Add("Player added to tournament successfully!", Severity.Success);
            MudDialog.Close(DialogResult.Ok(true));
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error adding player: {ex.Message}", Severity.Error);
            _isSaving = false;
        }
    }

    private void Cancel()
    {
        MudDialog.Close(DialogResult.Cancel());
    }
}

