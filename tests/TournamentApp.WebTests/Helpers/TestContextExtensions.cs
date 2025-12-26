using Bunit;
using Bunit.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;
using NSubstitute;
using TournamentApp.Web.Contracts.Services;
using TournamentApp.Web.Services;

namespace TournamentApp.WebTests.Helpers;

public static class TestContextExtensions
{
    public static TestContext AddMudBlazorServices(this TestContext ctx)
    {
        ctx.Services.AddMudServices();
        ctx.JSInterop.Mode = JSRuntimeMode.Loose;
        return ctx;
    }

    public static IRenderedFragment RenderWithMudPopoverProvider(this TestContext ctx, RenderFragment childContent)
    {
        return ctx.Render(builder =>
        {
            builder.OpenComponent<MudPopoverProvider>(0);
            builder.AddAttribute(1, "ChildContent", childContent);
            builder.CloseComponent();
        });
    }

    public static TestContext AddMockServices(this TestContext ctx)
    {
        ctx.Services.AddSingleton<IPlayerService>(_ => Substitute.For<IPlayerService>());
        ctx.Services.AddSingleton<ITournamentService>(_ => Substitute.For<ITournamentService>());
        ctx.Services.AddSingleton<IBracketService>(_ => Substitute.For<IBracketService>());
        ctx.Services.AddSingleton<IMatchService>(_ => Substitute.For<IMatchService>());
        return ctx;
    }

    public static TestContext AddRealServices(this TestContext ctx, HttpClient httpClient)
    {
        ctx.Services.AddSingleton<IPlayerService>(_ => new PlayerService(httpClient));
        ctx.Services.AddSingleton<ITournamentService>(_ => new TournamentService(httpClient));
        ctx.Services.AddSingleton<IBracketService>(_ => new BracketService(httpClient));
        ctx.Services.AddSingleton<IMatchService>(_ => new MatchService(httpClient));
        return ctx;
    }
}