using System.Reflection;
using FluentValidation;
using MediatR;
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

        return services;
    }
}
