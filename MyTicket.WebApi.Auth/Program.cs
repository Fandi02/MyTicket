using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using MyTicket.Application.Infrastructure;
using MyTicket.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MyTicketDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ServerConnectionAuth"))
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

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
