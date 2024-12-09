using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyTicket.Application.Constant;
using MyTicket.Application.Interfaces;
using MyTicket.Application.Models;
using MyTicket.Domain.Entities.Auth;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace MyTicket.Application.Services;

public class ConsumerUser : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ConnectionFactory _factory;

    public ConsumerUser(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _factory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "MyUser",
            Password = "MyPassword",
            VirtualHost = "/"
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var connection = _factory.CreateConnection();
        using var channel = connection.CreateModel();

        var exchangeName = "MyExchange";
        var queueName = "UserTransaction";
        
        channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout);
        channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);
        channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: "");

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            var userEvent = JsonConvert.DeserializeObject<UserModel>(message);

            if (userEvent != null)
            {
                ProcessEvent(userEvent, stoppingToken);
            }
        };

        channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

        // Ini akan menjaga listener tetap berjalan sampai service dihentikan
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);  // Delay kecil untuk menghindari polling intensif
        }
    }

    private void ProcessEvent(UserModel userEvent, CancellationToken stoppingToken)
    {
        switch (userEvent.EventType)
        {
            case EventTypeRabbitMq.CreateUser:
                HandleUserCreated(userEvent, stoppingToken);
                break;
            case EventTypeRabbitMq.UpdateUser:
                HandleUserUpdated(userEvent, stoppingToken);
                break;
        }
    }

    private async void HandleUserCreated(UserModel data, CancellationToken stoppingToken)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<IMyTicketDbContext>();

            var saveUser = new User
            {
                UserId = data.UserId,
                Email = data.Email,
                FullName = data.FullName,
                PhoneNumber = data.PhoneNumber,
                UserName = data.UserName,
                BirthDate = data.BirthDate,
                Role = data.Role,
                IsActivate = true
            };

            await dbContext.Users.AddAsync(saveUser);
            await dbContext.SaveChangesAsync(stoppingToken);
        }
    }

    private async void HandleUserUpdated(UserModel data, CancellationToken stoppingToken)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<IMyTicketDbContext>();

            var user = dbContext.Users.FirstOrDefault(u => u.UserId == data.UserId);

            if (user != null)
            {
                user.UserId = data.UserId;
                user.Email = data.Email;
                user.FullName = data.FullName;
                user.PhoneNumber = data.PhoneNumber;
                user.UserName = data.UserName;
                user.BirthDate = data.BirthDate;
                user.Role = data.Role;

                dbContext.Users.Update(user);
                await dbContext.SaveChangesAsync(stoppingToken);
            }
        }
    }
}