using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace TournamentApp.Web.Dialogs;

public partial class ConfirmDialog : ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; } = null!;
    
    [Parameter] public string Title { get; set; } = "Confirm";
    [Parameter] public string Message { get; set; } = string.Empty;
    [Parameter] public string ConfirmText { get; set; } = "Confirm";
    [Parameter] public string CancelText { get; set; } = "Cancel";
    [Parameter] public Color ConfirmColor { get; set; } = Color.Primary;

    private void Confirm()
    {
        MudDialog.Close(DialogResult.Ok(true));
    }

    private void Cancel()
    {
        MudDialog.Close(DialogResult.Cancel());
    }
}

