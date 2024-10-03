using System.Reflection;
using FluentValidation;
using MediatR;
using MyTicket.Application.Infrastructure;
using MyTicket.Application.Interfaces;
using MyTicket.Application.JwtBearer;
using MyTicket.Application.Models;
using MyTicket.Application.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddMediatR(Assembly.GetExecutingAssembly());
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddTransient<JwtBearerService>();
        services.AddTransient<JwtConfig>();
        services.AddTransient<ApplicationJwtManagerService>();
        services.AddTransient<BaseAuthenticatedUser>();
        services.AddTransient<IContext, ContextService>();
        services.AddTransient<IClock, ClockSevice>();
        services.AddTransient<ClockOptions>();

        return services;
    }
}
