using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using MyTicket.Application.Infrastructure;
using MyTicket.WebApi.Services;

namespace MyTicket.WebApi.Common;

public static class SwaggerConfiguration
{
    public static void AddSwaggerGen2(this IServiceCollection services)
    {
        var options = services.GetOptions<SwaggerOptions>("auth");

        services.AddSingleton(options);
        services.AddSwaggerGen(swagger =>
        {
            swagger.MapType<DateOnly>(() => new OpenApiSchema
            {
                Type = "string",
                Format = "date",
                Example = new OpenApiString("2022-01-01")
            });

            swagger.EnableAnnotations();
            swagger.OperationFilter<AddAuthorizationHeaderOperationFilter>();
            swagger.CustomSchemaIds(x => x.FullName);
            swagger.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = options.Title,
                Version = options.Version
            });
            swagger.AddSecurityDefinition(
                name: "Bearer",
                securityScheme: new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Description =
                        "Input your Bearer token in this format - Bearer {your token here} to access this API",
                });
        });
    }
}