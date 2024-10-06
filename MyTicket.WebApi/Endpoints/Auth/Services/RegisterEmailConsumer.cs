using Microsoft.AspNetCore.Identity.UI.Services;
using MyTicket.WebApi.Endpoints.Auth.Models.Request;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace MyTicket.WebApi.Endpoints.Auth.Services;

public class RegisterEmailConsumer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly ConnectionFactory _factory;

    public RegisterEmailConsumer(IServiceProvider serviceProvider, IConfiguration configuration)
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
        
        channel.QueueDeclare(queue: "email_queue", durable: false, exclusive: false, autoDelete: true, arguments: null);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += async (model, ea) =>
        {
            using (var scope = _serviceProvider.CreateScope())  // Membuat scope baru
            {
                var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();  // Mengambil IEmailSender scoped service

                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var registerModelRequest = JsonConvert.DeserializeObject<RegisterModelRequest>(message);

                if (registerModelRequest != null)
                {
                    var toEmail = registerModelRequest.Email;
                    var subject = "Welcome to MyTicket";
                    var bodyText = $"Hello {registerModelRequest.FullName}, thank you for registering!";

                    // Kirim email
                    await emailSender.SendEmailAsync(toEmail, subject, bodyText);
                }
            }
        };

        channel.BasicConsume(queue: "email_queue", autoAck: true, consumer: consumer);

        // Ini akan menjaga listener tetap berjalan sampai service dihentikan
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);  // Delay kecil untuk menghindari polling intensif
        }
    }
}