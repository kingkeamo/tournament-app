using FluentValidation.Results;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
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
    private EditContext _editContext = null!;
    private List<PlayerDto>? _players;
    private bool _isSaving;
    private IReadOnlyCollection<Guid> _selectedPlayerIds = new HashSet<Guid>();

    protected override async Task OnInitializedAsync()
    {
        _viewModel.TournamentId = TournamentId;
        _viewModel.PlayerIds = new List<Guid>();
        _selectedPlayerIds = new HashSet<Guid>();
        _editContext = new EditContext(_viewModel);
        await LoadPlayers();
    }

    private async Task HandleSubmit()
    {
        _viewModel.PlayerIds = _selectedPlayerIds.ToList();
        
        if (!_editContext.Validate())
        {
            StateHasChanged();
            return;
        }

        await HandleValidSubmit();
    }
    
    private void OnSelectedValuesChanged(IReadOnlyCollection<Guid> selectedValues)
    {
        _selectedPlayerIds = selectedValues;
        _viewModel.PlayerIds = selectedValues.ToList();
        _editContext.NotifyFieldChanged(_editContext.Field(nameof(_viewModel.PlayerIds)));
        StateHasChanged();
    }

    private async Task LoadPlayers()
    {
        try
        {
            // Load all players
            var playersResponse = await PlayerService.GetPlayers();
            if (playersResponse.IsSuccess && playersResponse.Data != null)
            {
                var allPlayers = playersResponse.Data.ToList();
                
                // Load tournament to get current players
                var tournamentResponse = await TournamentService.GetTournament(TournamentId);
                if (tournamentResponse.IsSuccess && tournamentResponse.Data != null)
                {
                    var currentPlayerIds = tournamentResponse.Data.PlayerIds?.ToList() ?? new List<Guid>();
                    // Filter out players already in the tournament
                    _players = allPlayers.Where(p => !currentPlayerIds.Contains(p.Id)).ToList();
                }
                else
                {
                    _players = allPlayers;
                }
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

            var playerCount = _viewModel.PlayerIds?.Count ?? 0;
            var message = playerCount == 1 
                ? "Player added to tournament successfully!" 
                : $"{playerCount} players added to tournament successfully!";
            Snackbar.Add(message, Severity.Success);
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

