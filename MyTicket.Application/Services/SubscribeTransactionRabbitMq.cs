using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyTicket.Application.Constant;
using MyTicket.Application.Interfaces;
using MyTicket.Application.Models;
using MyTicket.Domain.Entities.Auth;
using MyTicket.Domain.Entities.Transaction;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace MyTicket.Application.Services;

public class ConsumerTransaction : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ConnectionFactory _factory;

    public ConsumerTransaction(IServiceProvider serviceProvider)
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

        var exchangeName = "ExchangeTransaction";
        var queueName = "TransactionPayment";
        
        channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout);
        channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);
        channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: "");

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            var transactionPayment = JsonConvert.DeserializeObject<TransactionModel>(message);

            if (transactionPayment != null)
            {
                ProcessEvent(transactionPayment, stoppingToken);
            }
        };

        channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

        // Ini akan menjaga listener tetap berjalan sampai service dihentikan
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);  // Delay kecil untuk menghindari polling intensif
        }
    }

    private void ProcessEvent(TransactionModel transactionPayment, CancellationToken stoppingToken)
    {
        switch (transactionPayment.EventType)
        {
            case EventTypeRabbitMq.OrderCreated:
                HandleOrderCreated(transactionPayment, stoppingToken);
                break;
            case EventTypeRabbitMq.OrderUpdated:
                HandleOrderUpdated(transactionPayment, stoppingToken);
                break;
            case EventTypeRabbitMq.CancelOrder:
                HandleCancelOrder(transactionPayment, stoppingToken);
                break;
        }
    }

    private async void HandleOrderCreated(TransactionModel data, CancellationToken stoppingToken)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<IMyTicketDbContext>();

            var eventData = dbContext.Events.FirstOrDefault(u => u.EventId == data.EventId);

            if (eventData == null)
            {
                var saveEvent = new Event
                {
                    EventId = data.EventId,
                    Name = data.Name,
                    Description = data.Description,
                    StartDate = data.StartDate,
                    EndDate = data.EndDate,
                    TotalTicket = data.TotalTicket,
                    AvailableTicket = data.AvailableTicket,
                    Price = data.Price,
                    Location = data.Location
                };

                await dbContext.Events.AddAsync(saveEvent);
            }

            var saveOrderTicket = new OrderTicket
            {
                OrderTicketId = data.OrderTicketId,
                UserId = data.UserId,
                EventId = data.EventId,
                TicketNumber = data.TicketNumber,
                Quantity = data.Quantity,
                Date = data.Date,
                IsPaid = data.IsPaid
            };

            await dbContext.OrderTickets.AddAsync(saveOrderTicket);
            await dbContext.SaveChangesAsync(stoppingToken);
        }
    }

    private async void HandleOrderUpdated(TransactionModel data, CancellationToken stoppingToken)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<IMyTicketDbContext>();

            var eventData = dbContext.Events.FirstOrDefault(u => u.EventId == data.EventId);

            if (eventData != null)
            {
                eventData.EventId = data.EventId;
                eventData.Name = data.Name;
                eventData.Description = data.Description;
                eventData.StartDate = data.StartDate;
                eventData.EndDate = data.EndDate;
                eventData.TotalTicket = data.TotalTicket;
                eventData.AvailableTicket = data.AvailableTicket;
                eventData.Price = data.Price;
                eventData.Location = data.Location;

                dbContext.Events.Update(eventData);
                await dbContext.SaveChangesAsync(stoppingToken);
            }

            var orderTicketData = dbContext.OrderTickets.FirstOrDefault(u => u.OrderTicketId == data.OrderTicketId);

            if (orderTicketData != null)
            {
                orderTicketData.OrderTicketId = data.OrderTicketId;
                orderTicketData.UserId = data.UserId;
                orderTicketData.EventId = data.EventId;
                orderTicketData.TicketNumber = data.TicketNumber;
                orderTicketData.Quantity = data.Quantity;
                orderTicketData.Date = data.Date;
                orderTicketData.IsPaid = data.IsPaid;

                dbContext.OrderTickets.Update(orderTicketData);
                await dbContext.SaveChangesAsync(stoppingToken);
            }
        }
    }

    private async void HandleCancelOrder(TransactionModel data, CancellationToken stoppingToken)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<IMyTicketDbContext>();

            var eventData = dbContext.Events.FirstOrDefault(u => u.EventId == data.EventId);

            if (eventData != null)
            {
                eventData.EventId = data.EventId;
                eventData.Name = data.Name;
                eventData.Description = data.Description;
                eventData.StartDate = data.StartDate;
                eventData.EndDate = data.EndDate;
                eventData.TotalTicket = data.TotalTicket;
                eventData.AvailableTicket = data.AvailableTicket;
                eventData.Price = data.Price;
                eventData.Location = data.Location;

                dbContext.Events.Update(eventData);
                await dbContext.SaveChangesAsync(stoppingToken);
            }

            var orderTicketData = dbContext.OrderTickets.FirstOrDefault(u => u.OrderTicketId == data.OrderTicketId);

            if (orderTicketData != null)
            {
                dbContext.OrderTickets.Remove(orderTicketData);
                await dbContext.SaveChangesAsync(stoppingToken);
            }
        }
    }
}