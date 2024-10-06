using Microsoft.AspNetCore.Identity.UI.Services;
using MyTicket.WebApi.Endpoints.Event.Models.Service;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace MyTicket.WebApi.Endpoints.Event.Services;

public class CreateEventConsumer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly ConnectionFactory _factory;

    public CreateEventConsumer(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
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
        
        channel.QueueDeclare(queue: "create-event", durable: false, exclusive: false, autoDelete: true, arguments: null);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += async (model, ea) =>
        {
            using (var scope = _serviceProvider.CreateScope())  // Membuat scope baru
            {
                var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();  // Mengambil IEmailSender scoped service

                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var createEventRequest = JsonConvert.DeserializeObject<EventConsumerModel>(message);

                if (createEventRequest != null && createEventRequest.Email != null)
                {
                    var toEmail = createEventRequest.Email;
                    var subject = "Information From MyTicket";
                    var bodyText = $"Hello {createEventRequest.FullName}, event {createEventRequest.EventName} has been created!. {createEventRequest.Description} will be held on {createEventRequest.StartDate.ToString("dd/MM/yyyy")} - {createEventRequest.EndDate.ToString("dd/MM/yyyy")} at {createEventRequest.Location}.";

                    // Kirim email
                    await emailSender.SendEmailAsync(toEmail, subject, bodyText);
                }
            }
        };

        channel.BasicConsume(queue: "create-event", autoAck: true, consumer: consumer);

        // Ini akan menjaga listener tetap berjalan sampai service dihentikan
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);  // Delay kecil untuk menghindari polling intensif
        }
    }
}