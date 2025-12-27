using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TournamentApp.Application.Common.Behaviours;

namespace TournamentApp.Application;

public static class DependencyInjection
{
    public static void AddTournamentAppApplication(this IServiceCollection services)
    {
        services.AddTournamentAppMediator();
    }
    
    private static void AddTournamentAppMediator(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
    }
}

