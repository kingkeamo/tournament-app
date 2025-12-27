using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using TournamentApp.Web;
using TournamentApp.Web.Contracts.Services;
using TournamentApp.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Explicitly load environment-specific appsettings files
// Note: Blazor WebAssembly loads these from wwwroot via HTTP at runtime
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
builder.Configuration.AddJsonFile($"appsettings.{builder.HostEnvironment.Environment}.json", optional: true, reloadOnChange: false);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure API base URL from appsettings.json
// Note: Blazor WebAssembly runs in the browser, so user secrets are not available.
// Configuration must come from appsettings.json (which must be in wwwroot folder)
var apiBaseUrl = builder.Configuration["ApiBaseUrl"];

// Validate that API URL is configured
if (string.IsNullOrWhiteSpace(apiBaseUrl))
{
    throw new InvalidOperationException(
        "ApiBaseUrl is not configured. Please set it in appsettings.json.");
}

// Ensure API URL is not the same as frontend URL
if (apiBaseUrl == builder.HostEnvironment.BaseAddress)
{
    throw new InvalidOperationException(
        $"ApiBaseUrl cannot be the same as the frontend BaseAddress ({builder.HostEnvironment.BaseAddress}). " +
        "Please configure a different API URL in appsettings.json.");
}

// Ensure BaseAddress ends with a trailing slash
if (!apiBaseUrl.EndsWith("/"))
{
    apiBaseUrl += "/";
}

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });

// Register services
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<ITournamentService, TournamentService>();
builder.Services.AddScoped<IBracketService, BracketService>();
builder.Services.AddScoped<IMatchService, MatchService>();

// Add MudBlazor
builder.Services.AddMudServices();

await builder.Build().RunAsync();
