using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using MyTicket.Application.Infrastructure;
using MyTicket.Application.Infrastructure.SystemTextJson;
using MyTicket.Application.Interfaces;
using MyTicket.Persistence;
using MyTicket.WebApi.Common;
using MyTicket.WebApi.Endpoints.Auth.Services;
using MyTicket.WebApi.Endpoints.Event.Services;
using MyTicket.WebApi.Endpoints.UserProfile.Services;
using MyTicket.WebApi.ServiceMessageBroker;
using MyTicket.WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MyTicketDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ServerConnection"))
);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.Filters.Add(new ResultManipulator());

    // add slugify dash in route controller (FooBarController --> foo-bar)
    options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
});

builder.Services.AddApplicationServices();

builder.Services.AddTransient<ApplicationJwtManager>();
builder.Services.AddScoped<IMyTicketDbContext, MyTicketDbContext>();
builder.Services.AddScoped<IMessageProducer, MessageProducer>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddHostedService<RegisterEmailConsumer>();
builder.Services.AddHostedService<UpdatePasswordConsumer>();
builder.Services.AddHostedService<CreateEventConsumer>();
builder.Services.AddHostedService<UpdateEventConsumer>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddTransient<ExceptionHandlerMiddlewareOptions>();

builder.Services.AddAuthentication(o =>
{
    o.DefaultScheme = CustomJwtAuthenticationOptions.DefaultSchemeName;
    o.DefaultAuthenticateScheme = CustomJwtAuthenticationOptions.DefaultSchemeName;
})
.AddScheme<CustomJwtAuthenticationOptions, CustomJwtAuthenticationHandler>(CustomJwtAuthenticationOptions.DefaultSchemeName, opts => { });

builder.Services.AddAuthorization(opts =>
{
    opts.DefaultPolicy = new AuthorizationPolicyBuilder()
    .AddAuthenticationSchemes(CustomJwtAuthenticationOptions.DefaultSchemeName)
    .RequireAuthenticatedUser()
    .Build();
});

builder.Services.AddControllers(options =>
    {
        options.Conventions.Add(
            new CustomRouteToken(
                "namespace",
                c => c.ControllerType.Namespace?.Split('.').Last()
            ));
        options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new DateOnlyConverter());
        options.JsonSerializerOptions.Converters.Add(new TimeOnlyConverter());
    });

builder.Services.AddSwaggerGen2();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCustomExceptionHandler();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
