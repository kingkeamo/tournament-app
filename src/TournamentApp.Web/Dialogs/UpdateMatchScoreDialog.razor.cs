using FluentValidation.Results;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using TournamentApp.Web.Contracts.Services;
using TournamentApp.Web.Responses;

namespace TournamentApp.Web.Dialogs;

public partial class UpdateMatchScoreDialog : ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; } = null!;
    [Parameter] public Guid MatchId { get; set; }
    [Inject] protected IMatchService MatchService { get; set; } = null!;
    [Inject] protected ISnackbar Snackbar { get; set; } = null!;

    private UpdateMatchScoreViewModel _viewModel = new();
    private EditContext _editContext = null!;
    private bool _isSaving;

    protected override void OnInitialized()
    {
        _viewModel.MatchId = MatchId;
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
        if (_viewModel.Score1 == _viewModel.Score2)
        {
            Snackbar.Add("Scores cannot be equal. One player must win.", Severity.Warning);
            return;
        }

        _isSaving = true;
        
        try
        {
            var response = await MatchService.UpdateMatchScore(MatchId, _viewModel);

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

            Snackbar.Add("Match score updated successfully!", Severity.Success);
            MudDialog.Close(DialogResult.Ok(true));
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error updating score: {ex.Message}", Severity.Error);
            _isSaving = false;
        }
    }

    private void Cancel()
    {
        MudDialog.Close(DialogResult.Cancel());
    }
}

