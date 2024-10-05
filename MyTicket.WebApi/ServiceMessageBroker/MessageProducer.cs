using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace MyTicket.WebApi.ServiceMessageBroker;

public class MessageProducer : IMessageProducer
{
    private readonly ConnectionFactory _factory;
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

        channel.QueueDeclare(queueName, durable: false, exclusive: false);

        var jsonString = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(jsonString);

         channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
    }
}