using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using TournamentApp.Infrastructure;
using TournamentApp.Infrastructure.DbUp;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.Load("TournamentApp.Application")));

// Add Infrastructure (repositories, DbUp, etc.)
builder.Services.AddInfrastructure(builder.Configuration);

// Configure CORS for GitHub Pages
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowGitHubPages", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Configure JWT Authentication
var cognitoAuthority = builder.Configuration["Cognito:Authority"];
var cognitoUserPoolId = builder.Configuration["Cognito:UserPoolId"];

if (!string.IsNullOrEmpty(cognitoAuthority) && !string.IsNullOrEmpty(cognitoUserPoolId))
{
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.Authority = cognitoAuthority;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true
            };
        });
}

var app = builder.Build();

// Run database migrations
using (var scope = app.Services.CreateScope())
{
    var migrator = scope.ServiceProvider.GetRequiredService<DatabaseMigrator>();
    var result = migrator.Migrate();
    if (!result.Successful)
    {
        throw new Exception("Database migration failed", result.Error);
    }
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowGitHubPages");
app.UseAuthentication();
app.UseAuthorization();

// Add a simple test endpoint to verify CORS
app.MapGet("/test-cors", () => new { message = "CORS is working!" })
    .WithName("TestCors")
    .RequireCors("AllowGitHubPages");
app.MapControllers();

app.Run();
