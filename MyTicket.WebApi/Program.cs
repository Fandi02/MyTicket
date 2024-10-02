using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using MyTicket.Application.Infrastructure;
using MyTicket.Application.Interfaces;
using MyTicket.Application.Services;
using MyTicket.Persistence;
using MyTicket.WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MyTicketDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ServerConnection"))
);

builder.Services.AddSingleton<ExceptionHandlerMiddlewareOptions>();

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

builder.Services.AddTransient<IContext, ContextService>();
builder.Services.AddTransient<IClock, ClockSevice>();
builder.Services.AddTransient<ClockOptions>();
builder.Services.AddTransient<ApplicationJwtManager>();
builder.Services.AddScoped<IMyTicketDbContext, MyTicketDbContext>();

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.Filters.Add(new ResultManipulator());

    // add slugify dash in route controller (FooBarController --> foo-bar)
    options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplicationServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCustomExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
