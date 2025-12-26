using Bunit;
using MudBlazor;
using TournamentApp.WebTests.Helpers; // <-- this is where your AddMudBlazorServices() extension lives

namespace TournamentApp.WebTests.Helpers;

public abstract class MudBlazorTestBase : TestContext
{
    protected MudBlazorTestBase()
    {
        // Uses your existing helper that already compiles in your solution
        this.AddMudBlazorServices();
    }

    /// <summary>
    /// Render the minimum required MudBlazor providers for dialogs + selects (popovers).
    /// Call this AFTER registering your mocks in the derived test constructor.
    /// </summary>
    protected IRenderedFragment RenderMudBlazorHost()
    {
        return Render(builder =>
        {
            builder.OpenComponent<MudThemeProvider>(0);

            builder.OpenComponent<MudPopoverProvider>(1);
            builder.CloseComponent();

            builder.OpenComponent<MudDialogProvider>(2);
            builder.CloseComponent();

            builder.CloseComponent();
        });
    }
}