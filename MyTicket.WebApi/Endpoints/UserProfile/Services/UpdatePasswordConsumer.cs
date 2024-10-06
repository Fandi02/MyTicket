using Microsoft.AspNetCore.Identity.UI.Services;
using MyTicket.WebApi.Endpoints.UserProfile.Models.Service;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace MyTicket.WebApi.Endpoints.UserProfile.Services;

public class UpdatePasswordConsumer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly ConnectionFactory _factory;

    public UpdatePasswordConsumer(IServiceProvider serviceProvider, IConfiguration configuration)
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
        
        channel.QueueDeclare(queue: "email_queue_update_password", durable: false, exclusive: false, autoDelete: true, arguments: null);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += async (model, ea) =>
        {
            using (var scope = _serviceProvider.CreateScope())  // Membuat scope baru
            {
                var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();  // Mengambil IEmailSender scoped service

                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var updatePasswordModelRequest = JsonConvert.DeserializeObject<UpdatePasswordServiceModel>(message);

                if  (updatePasswordModelRequest != null && updatePasswordModelRequest.Email != null)
                {
                    var toEmail = updatePasswordModelRequest.Email;
                    var subject = "MyTicket APP";
                    var bodyText = $"Hello  {updatePasswordModelRequest.FullName}, your password has been updated!";

                    // Kirim email
                    await emailSender.SendEmailAsync(toEmail, subject, bodyText);
                }
            }
        };

        channel.BasicConsume(queue: "email_queue_update_password", autoAck: true, consumer: consumer);

        // Ini akan menjaga listener tetap berjalan sampai service dihentikan
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);  // Delay kecil untuk menghindari polling intensif
        }
    }
}