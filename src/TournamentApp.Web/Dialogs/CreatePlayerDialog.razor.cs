using FluentValidation.Results;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using TournamentApp.Web.Contracts.Services;
using TournamentApp.Web.Responses;

namespace TournamentApp.Web.Dialogs;

public partial class CreatePlayerDialog : ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; } = null!;
    [Inject] protected IPlayerService PlayerService { get; set; } = null!;
    [Inject] protected ISnackbar Snackbar { get; set; } = null!;

    private AddPlayerViewModel _viewModel = new();
    private EditContext _editContext = null!;
    private bool _isSaving;

    protected override void OnInitialized()
    {
        _editContext = new EditContext(_viewModel);
    }

    private async Task HandleSubmit()
    {
        if (!_editContext.Validate())
        {
            StateHasChanged();
            return;
        }

        await HandleValidSubmit();
    }

    private async Task HandleValidSubmit()
    {
        _isSaving = true;
        
        try
        {
            var response = await PlayerService.AddPlayer(_viewModel);

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

            Snackbar.Add("Player created successfully!", Severity.Success);
            MudDialog.Close(DialogResult.Ok(response.NewId));
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error creating player: {ex.Message}", Severity.Error);
            _isSaving = false;
        }
    }

    private void Cancel()
    {
        MudDialog.Close(DialogResult.Cancel());
    }
}

