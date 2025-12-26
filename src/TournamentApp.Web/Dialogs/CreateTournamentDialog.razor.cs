using FluentValidation.Results;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using TournamentApp.Web.Contracts.Services;
using TournamentApp.Web.Responses;

namespace TournamentApp.Web.Dialogs;

public partial class CreateTournamentDialog : ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; } = null!;
    [Inject] protected ITournamentService TournamentService { get; set; } = null!;
    [Inject] protected ISnackbar Snackbar { get; set; } = null!;

    private CreateTournamentViewModel _viewModel = new();
    private bool _isSaving;

    private async Task HandleValidSubmit()
    {
        _isSaving = true;
        
        try
        {
            var response = await TournamentService.CreateTournament(_viewModel);

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

            Snackbar.Add("Tournament created successfully!", Severity.Success);
            MudDialog.Close(DialogResult.Ok(response.NewId));
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error creating tournament: {ex.Message}", Severity.Error);
            _isSaving = false;
        }
    }

    private void Cancel()
    {
        MudDialog.Close(DialogResult.Cancel());
    }
}

