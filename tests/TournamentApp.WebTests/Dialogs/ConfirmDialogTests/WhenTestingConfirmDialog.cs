using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using TournamentApp.Web.Dialogs;
using TournamentApp.WebTests.Helpers;

namespace TournamentApp.WebTests.Dialogs.ConfirmDialogTests;

public class WhenTestingConfirmDialog : TestContext
{
    public WhenTestingConfirmDialog()
    {
        this.AddMudBlazorServices();
    }

    [Fact]
    public async Task ItShouldDisplayTitleWhenProvided()
    {
        var title = "Test Title";
        var dialogService = Services.GetRequiredService<IDialogService>();
        var dialogProvider = RenderComponent<MudDialogProvider>();
        
        var dialogReference = await dialogService.ShowAsync<ConfirmDialog>(title, new DialogParameters
        {
            { "Title", title },
            { "Message", "Test message" }
        });

        dialogProvider.WaitForAssertion(() =>
        {
            dialogProvider.Markup.Should().Contain(title);
        }, timeout: TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task ItShouldDisplayMessageWhenProvided()
    {
        var message = "Test message";
        var dialogService = Services.GetRequiredService<IDialogService>();
        var dialogProvider = RenderComponent<MudDialogProvider>();
        
        var dialogReference = await dialogService.ShowAsync<ConfirmDialog>("", new DialogParameters
        {
            { "Message", message }
        });

        dialogProvider.WaitForAssertion(() =>
        {
            dialogProvider.Markup.Should().Contain(message);
        }, timeout: TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task ItShouldDisplayDefaultTitleWhenNotProvided()
    {
        var dialogService = Services.GetRequiredService<IDialogService>();
        var dialogProvider = RenderComponent<MudDialogProvider>();
        
        var dialogReference = await dialogService.ShowAsync<ConfirmDialog>("");

        dialogProvider.WaitForAssertion(() =>
        {
            dialogProvider.Markup.Should().Contain("Confirm");
        }, timeout: TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task ItShouldDisplayDefaultConfirmTextWhenNotProvided()
    {
        var dialogService = Services.GetRequiredService<IDialogService>();
        var dialogProvider = RenderComponent<MudDialogProvider>();
        
        var dialogReference = await dialogService.ShowAsync<ConfirmDialog>("", new DialogParameters
        {
            { "Message", "Test" }
        });

        dialogProvider.WaitForAssertion(() =>
        {
            dialogProvider.Markup.Should().Contain("Confirm");
        }, timeout: TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task ItShouldDisplayDefaultCancelTextWhenNotProvided()
    {
        var dialogService = Services.GetRequiredService<IDialogService>();
        var dialogProvider = RenderComponent<MudDialogProvider>();
        
        var dialogReference = await dialogService.ShowAsync<ConfirmDialog>("", new DialogParameters
        {
            { "Message", "Test" }
        });

        dialogProvider.WaitForAssertion(() =>
        {
            dialogProvider.Markup.Should().Contain("Cancel");
        }, timeout: TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task ItShouldDisplayCustomConfirmTextWhenProvided()
    {
        var confirmText = "Yes, Delete";
        var dialogService = Services.GetRequiredService<IDialogService>();
        var dialogProvider = RenderComponent<MudDialogProvider>();
        
        var dialogReference = await dialogService.ShowAsync<ConfirmDialog>("", new DialogParameters
        {
            { "Message", "Test" },
            { "ConfirmText", confirmText }
        });

        dialogProvider.WaitForAssertion(() =>
        {
            dialogProvider.Markup.Should().Contain(confirmText);
        }, timeout: TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task ItShouldDisplayCustomCancelTextWhenProvided()
    {
        var cancelText = "No, Keep";
        var dialogService = Services.GetRequiredService<IDialogService>();
        var dialogProvider = RenderComponent<MudDialogProvider>();
        
        var dialogReference = await dialogService.ShowAsync<ConfirmDialog>("", new DialogParameters
        {
            { "Message", "Test" },
            { "CancelText", cancelText }
        });

        dialogProvider.WaitForAssertion(() =>
        {
            dialogProvider.Markup.Should().Contain(cancelText);
        }, timeout: TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task ItShouldReturnOkWhenConfirmButtonIsClicked()
    {
        var dialogService = Services.GetRequiredService<IDialogService>();
        var dialogProvider = RenderComponent<MudDialogProvider>();
        
        var dialogReference = await dialogService.ShowAsync<ConfirmDialog>("", new DialogParameters
        {
            { "Message", "Test" }
        });

        dialogProvider.WaitForAssertion(() =>
        {
            dialogProvider.Markup.Should().Contain("Confirm");
        }, timeout: TimeSpan.FromSeconds(5));

        var buttons = dialogProvider.FindAll("button");
        var confirmButton = buttons.FirstOrDefault(b => b.TextContent.Contains("Confirm"));
        confirmButton.Should().NotBeNull();

        var resultTask = dialogReference.Result;
        confirmButton!.Click();

        await Task.Delay(200);

        var result = await resultTask;
        result.Canceled.Should().BeFalse();
        result.Data.Should().Be(true);
    }

    [Fact]
    public async Task ItShouldReturnCancelWhenCancelButtonIsClicked()
    {
        var dialogService = Services.GetRequiredService<IDialogService>();
        var dialogProvider = RenderComponent<MudDialogProvider>();
        
        var dialogReference = await dialogService.ShowAsync<ConfirmDialog>("", new DialogParameters
        {
            { "Message", "Test" }
        });

        dialogProvider.WaitForAssertion(() =>
        {
            dialogProvider.Markup.Should().Contain("Cancel");
        }, timeout: TimeSpan.FromSeconds(5));

        var buttons = dialogProvider.FindAll("button");
        var cancelButton = buttons.FirstOrDefault(b => b.TextContent.Contains("Cancel"));
        cancelButton.Should().NotBeNull();

        var resultTask = dialogReference.Result;
        cancelButton!.Click();

        await Task.Delay(200);

        var result = await resultTask;
        result.Canceled.Should().BeTrue();
    }
}

