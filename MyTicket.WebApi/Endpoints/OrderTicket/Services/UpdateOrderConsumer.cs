using MediatR;
using MyTicket.Application.Businesses.OrderTicket.Commands;
using MyTicket.WebApi.Endpoints.OrderTicket.Models.Service;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace MyTicket.WebApi.Endpoints.OrderTicket.Services;

public class UpdateOrderConsumer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ConnectionFactory _factory;

    public UpdateOrderConsumer(IServiceProvider serviceProvider)
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
        
        channel.QueueDeclare(queue: "update-order", durable: false, exclusive: false, autoDelete: true, arguments: null);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            var updateOrderRequest = JsonConvert.DeserializeObject<UpdateOrderModel>(message);

            if (updateOrderRequest != null)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    await mediator.Send(new UpdateOrderCommand
                                        { 
                                            OrderTicketId = updateOrderRequest.OrderTicketId,
                                            Quantity = updateOrderRequest.Quantity
                                        });
                }
            }
        };

        channel.BasicConsume(queue: "update-order", autoAck: true, consumer: consumer);

        // Ini akan menjaga listener tetap berjalan sampai service dihentikan
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);  // Delay kecil untuk menghindari polling intensif
        }
    }
}