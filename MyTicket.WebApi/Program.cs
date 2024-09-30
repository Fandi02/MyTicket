using Microsoft.EntityFrameworkCore;
using MyTicket.Application.Interfaces;
using MyTicket.Application.Services;
using MyTicket.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MyTicketDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ServerConnection"))
);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

builder.Services.AddTransient<IContext, ContextService>();
builder.Services.AddTransient<IClock, ClockSevice>();
builder.Services.AddTransient<ClockOptions>();
builder.Services.AddScoped<IMyTicketDbContext, MyTicketDbContext>();

// Add services to the container.

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
