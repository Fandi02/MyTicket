using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using MyTicket.Application.Interfaces;
using RabbitMQ.Client;

namespace MyTicket.Application.Services;

public class MessageProducer : IMessageProducer
{
    private readonly ConnectionFactory _factory;
    private readonly IConfiguration _configuration;
    public MessageProducer()
    {
        _factory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "MyUser",
            Password = "MyPassword",
            VirtualHost = "/"
        };
    }

    public void SendingMessage(string queueName, object message)
    {
        var conn = _factory.CreateConnection();

        using var channel = conn.CreateModel();

        var exchangeName = "MyExchange";
        channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout);

        channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);

        var jsonString = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(jsonString);

        channel.BasicPublish(exchange: exchangeName, routingKey: "", basicProperties: null, body: body);
    }
}