using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace MyTicket.Application.Infrastructure;

public static class SwaggerExtension
{
    public static IServiceCollection AddSwaggerVersioning(this IServiceCollection services)
    {
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();

        //services.AddApiVersioning(setup =>
        //{
        //    setup.DefaultApiVersion = new ApiVersion(1, 0);
        //    setup.AssumeDefaultVersionWhenUnspecified = true;
        //    setup.ReportApiVersions = true;
        //});

        //services.AddVersionedApiExplorer(setup =>
        //{
        //    setup.GroupNameFormat = "'v'VVV";
        //    setup.SubstituteApiVersionInUrl = true;
        //    setup.DefaultApiVersion = new ApiVersion(1, 0);
        //});

        services.AddSwaggerGen(c =>
        {
            c.OperationFilter<AddAuthorizationHeaderOperationFilter>();
            c.OperationFilter<AddDefaultPaginationQueryParametersOperationFilter>();

            c.SwaggerDoc(
                name: "v1",
                info: new OpenApiInfo
                {
                    Title = "API Auth",
                    Version = "v1",
                    Description = "Api Project created by RadyaLabs",
                    Contact = new OpenApiContact
                    {
                        Name = "Radya Labs HQ",
                        Email = "info@radyalabs.com",
                        Url = new Uri("https://radyalabs.com/")
                    }
                });

            c.AddSecurityDefinition(
                name: "Bearer",
                securityScheme: new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Description = "Input your Bearer token in this format - Bearer {your token here} to access this API",
                });
        });

        return services;
    }
}
